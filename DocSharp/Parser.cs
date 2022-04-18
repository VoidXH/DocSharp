using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// Handles loading the code from files into nodes.
    /// </summary>
    public class Parser {
        readonly string path;
        readonly TreeNode root;
        readonly TreeView sourceInfo;
        readonly Font italic, underline;
        readonly string defines;
        readonly TaskEngine engine;

        /// <summary>
        /// Handles loading the code from files into nodes.
        /// </summary>
        /// <param name="path">Source folder path</param>
        /// <param name="root">Root node of the source tree</param>
        /// <param name="sourceInfo">Source code discovery tree</param>
        /// <param name="defines">Defined constants</param>
        /// <param name="engine">Task engine for process display</param>
        public Parser(string path, TreeNode root, TreeView sourceInfo, string defines, TaskEngine engine = null) {
            this.path = path;
            this.root = root;
            this.sourceInfo = sourceInfo;
            italic = new Font(sourceInfo.Font, FontStyle.Italic);
            underline = new Font(sourceInfo.Font, FontStyle.Underline);
            this.defines = defines;
            this.engine = engine;
        }

        /// <summary>
        /// Load the folder selected in the constructor.
        /// </summary>
        public void Process() {
            if (engine != null) {
                engine.UpdateProgressBar(0);
                engine.UpdateStatus("Searching for .cs files...");
            }
            string[] files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            int loc = 0;
            for (int i = 0; i < files.Length; ++i) {
                if (files[i].Contains("\\obj\\"))
                    continue;
                if (engine != null) {
                    engine.UpdateProgressBar(i * 100 / files.Length);
                    engine.UpdateStatusLazy($"Parsing {Path.GetFileName(files[i])} ({i * 100f / files.Length:0.00}%)...");
                }
                string text = File.ReadAllText(files[i]);
                loc += ParseBlock(text, root);
            }
            sourceInfo.Invoke((MethodInvoker)delegate { sourceInfo.EndUpdate(); });
            if (engine != null) {
                engine.UpdateProgressBar(100);
                engine.UpdateStatus($"Finished parsing {loc} lines of code!");
            }
        }

        /// <summary>
        /// Evaluate a preprocessor branch.
        /// </summary>
        /// <param name="parse">Condition</param>
        /// <param name="defineConstants">Constants that are true</param>
        bool ParseSimpleIf(string parse, string[] defineConstants) {
            int bracketPos;
            while ((bracketPos = parse.LastIndexOf('(')) != -1) {
                int endBracket = parse.IndexOf(')', bracketPos + 1);
                parse = parse.Substring(0, bracketPos) +
                    (ParseSimpleIf(parse.Substring(bracketPos + 1, endBracket - bracketPos - 1), defineConstants)
                    ? _true : _false) + parse.Substring(endBracket + 1);
            }
            string[] split = System.Text.RegularExpressions.Regex.Split(parse, splitRegex);
            int arrPos = 0, splitEnd = split.Length - 1;
            while (arrPos < splitEnd) {
                bool first = split[arrPos].Equals(_true) || Utils.ArrayContains(defineConstants, split[arrPos]),
                    second = split[arrPos + 2].Equals(_true) || Utils.ArrayContains(defineConstants, split[arrPos + 2]);
                split[arrPos + 2] = (split[arrPos + 1].Equals(_and) ? first && second : first || second) ? _true : _false;
                arrPos += 2;
            }
            return split[arrPos].Equals(_true);
        }

        /// <summary>
        /// Generate an element tree from code.
        /// </summary>
        /// <param name="code">Code</param>
        /// <param name="node">Root node</param>
        /// <returns>Lines of code parsed.</returns>
        int ParseBlock(string code, TreeNode node) {
            int codeLen = code.Length, lastEnding = 0, lastSlash = -2, lastEquals = -2,
                parenthesis = 0, depth = 0, lastRemovableDepth = 0, loc = 0;
            bool commentLine = false, inRemovableBlock = false, inString = false,
                preprocessorLine = false, preprocessorSkip = false,
                summaryLine = false, inTemplate = false, propertyArray = false;
            string summary = string.Empty;

            // Parse defines
            string[] defineConstants = defines.Split(';');
            int defineCount = defineConstants.Length;
            for (int i = 0; i < defineCount; ++i)
                defineConstants[i] = defineConstants[i].Trim();

            for (int i = 0; i < codeLen; ++i) {
                if (i != 0 && code[i - 1] != '\\') {
                    // Skip strings
                    if (!inString && code[i] == '"')
                        inString = true;
                    else if (inString) {
                        if (code[i] == '"')
                            inString = false;
                        else
                            continue;
                        // Skip characters
                    } else if (code[i] == '\'') {
                        if (code[i + 3] == '\'')
                            i += 4;
                        if (code[i + 2] == '\'')
                            i += 3;
                    }
                }
                // Actual parser
                switch (code[i]) {
                    case ',':
                    case ';':
                    case '{':
                    case '}':
                        if (!commentLine && !preprocessorSkip && parenthesis == 0) {
                            if (code[i] == ',' && inTemplate)
                                continue;
                            inTemplate = false;
                            bool instruction = code[i] == ';', block = code[i] == '{', closing = code[i] == '}';
                            if (block)
                                ++depth;
                            if (closing) {
                                --depth;
                                if (inRemovableBlock && depth < lastRemovableDepth) {
                                    inRemovableBlock = false;
                                    lastEnding = i;
                                    if (propertyArray) {
                                        ++lastEnding;
                                        propertyArray = false;
                                        break;
                                    }
                                }
                            }

                            string cutout = code.Substring(lastEnding, i - lastEnding).Trim();
                            lastEnding = i + 1;
                            if (inRemovableBlock) {
                                bool getter = cutout.EndsWith(_getter), setter = cutout.EndsWith(_setter);
                                if (getter || setter) {
                                    ElementInfo info = (ElementInfo)node.Tag;
                                    Visibility visHere = Visibility.Public;
                                    if (cutout.StartsWith(_protected)) visHere = Visibility.Protected;
                                    else if (cutout.StartsWith(_internal)) visHere = Visibility.Internal;
                                    else if (cutout.StartsWith(_private)) visHere = Visibility.Private;
                                    if (getter)
                                        info.Getter = visHere;
                                    else
                                        info.Setter = visHere;
                                    node.Tag = info;
                                }
                                continue;
                            }

                            // Remove multiline comments
                            int commentBlockStart;
                            while ((commentBlockStart = cutout.IndexOf(_commentBlockStart)) != -1) {
                                int commentEnd = cutout.IndexOf(_commentBlockEnd, commentBlockStart + 2) + 2;
                                if (commentEnd == 1)
                                    break;
                                cutout = cutout.Substring(0, commentBlockStart) + cutout.Substring(commentEnd);
                            }

                            if (cutout.StartsWith(_using))
                                break;

                            // Property array initialization
                            int nodes = node.Nodes.Count;
                            if (block && cutout[0] == '=' &&
                                nodes != 0 && ((ElementInfo)node.Nodes[nodes - 1].Tag).Kind == Element.Properties) {
                                lastRemovableDepth = depth;
                                inRemovableBlock = true;
                                propertyArray = true;
                                break;
                            }

                            if (cutout.Length > 0 && cutout[cutout.Length - 1] == '=')
                                cutout = cutout.Remove(cutout.Length - 2, 1).TrimEnd();
                            int lambda = cutout.IndexOf(_lambda);
                            if (lambda >= 0)
                                cutout = cutout.Substring(0, lambda);

                            // Attributes
                            string attributes = string.Empty;
                            while (cutout.StartsWith(_indexStart)) {
                                int endPos = cutout.IndexOf(_indexEnd);
                                string between = cutout.Substring(1, endPos - 1);
                                if (attributes.Equals(string.Empty))
                                    attributes = between;
                                else
                                    attributes = string.Format("{0}, {1}", attributes, between);
                                cutout = cutout.Substring(endPos + 1).TrimStart();
                            }

                            // Default value
                            string defaultValue = string.Empty;
                            int eqPos = -1;
                            while ((eqPos = cutout.IndexOf('=', eqPos + 1)) != -1) {
                                // Not a parameter
                                if (cutout.LastIndexOf('(', eqPos) == -1 || cutout.IndexOf(')', eqPos) == -1) {
                                    defaultValue = cutout.Substring(eqPos + 1).TrimStart();
                                    cutout = cutout.Substring(0, eqPos).TrimEnd();
                                    break;
                                }
                            }

                            // Visibility
                            Visibility vis = Visibility.Default;
                            if (Utils.RemoveModifier(ref cutout, _private)) vis = Visibility.Private;
                            else if (Utils.RemoveModifier(ref cutout, _protected)) vis = Visibility.Protected;
                            else if (Utils.RemoveModifier(ref cutout, _internal)) vis = Visibility.Internal;
                            else if (Utils.RemoveModifier(ref cutout, _public)) vis = Visibility.Public;

                            // Modifiers
                            string modifiers = string.Empty;
                            Utils.RemoveModifier(ref cutout, _partial_);
                            Utils.MoveModifiers(ref cutout, ref modifiers, Constants.modifiers);

                            // Type
                            string type = string.Empty;
                            int spaceIndex = -1;
                            while ((spaceIndex = cutout.IndexOf(' ', spaceIndex + 1)) != -1) {
                                // Not just the delegate word
                                if ((spaceIndex == -1 || !cutout.Substring(0, spaceIndex).Equals(_delegate)) &&
                                    // Not in a template type
                                    (cutout.LastIndexOf('<', spaceIndex) == -1 || cutout.IndexOf('>', spaceIndex) == -1)) {
                                    type = cutout.Substring(0, spaceIndex);
                                    if (type.IndexOf('(') != -1)
                                        type = constructor;
                                    else
                                        cutout = cutout.Substring(spaceIndex).TrimStart();
                                    break;
                                }
                            }
                            Element kind = Element.Variables;
                            if (type.Equals(_class)) kind = Element.Classes;
                            else if (type.Equals(_interface)) kind = Element.Interfaces;
                            else if (type.Equals(_namespace)) kind = Element.Namespaces;
                            else if (type.Equals(_enum)) kind = Element.Enums;
                            else if (type.Equals(_struct)) kind = Element.Structs;
                            else if (lambda >= 0) kind = cutout.Contains('(') ? Element.Functions : Element.Properties;

                            // Extension
                            string extends = string.Empty;
                            int extensionIndex = cutout.IndexOf(':');
                            if (extensionIndex != -1) {
                                extends = cutout.Substring(extensionIndex + 1).TrimStart();
                                cutout = cutout.Substring(0, extensionIndex).TrimEnd();
                            }

                            // Default visibility
                            if (vis == Visibility.Default && kind != Element.Namespaces) {
                                if (node.Tag != null && (((ElementInfo)node.Tag).Kind == Element.Enums ||
                                    ((ElementInfo)node.Tag).Kind == Element.Interfaces))
                                    vis = Visibility.Public;
                                else if (kind == Element.Classes || kind == Element.Interfaces || kind == Element.Structs)
                                    vis = Visibility.Internal;
                                else
                                    vis = Visibility.Private;
                            }

                            // Multiple ; support
                            if (instruction && cutout.Equals(string.Empty)) {
                                summary = string.Empty;
                                break;
                            }

                            // Node handling
                            TreeNode newNode = Utils.GetNodeByText(node, cutout);
                            if (!cutout.Equals(string.Empty)) {
                                if (newNode == null) {
                                    sourceInfo.Invoke((MethodInvoker)delegate {
                                        Utils.MakeNodeName(newNode = node.Nodes.Add(Utils.RemoveParamNames(cutout)), vis);
                                    });
                                    if (modifiers.Contains(_abstract))
                                        newNode.NodeFont = italic;
                                    else if (modifiers.Contains(_static))
                                        newNode.NodeFont = underline;
                                }
                                if (block) {
                                    node = newNode;
                                    if (kind == Element.Variables) {
                                        kind = cutout.IndexOf('(') != -1 ? Element.Functions : Element.Properties;
                                        lastRemovableDepth = depth;
                                        inRemovableBlock = true;
                                    }
                                }
                                if (newNode.Tag != null) {
                                    ElementInfo tag = (ElementInfo)newNode.Tag;
                                    tag.Summary += summary;
                                    newNode.Tag = tag;
                                }
                                // "int a, b;" case, copy tags
                                else if (type.Equals(string.Empty) && newNode.Parent.Nodes.Count > 1) {
                                    TreeNode lastNode = newNode.Parent.Nodes[newNode.Parent.Nodes.Count - 2];
                                    newNode.NodeFont = lastNode.NodeFont;
                                    ElementInfo inherited = (ElementInfo)lastNode.Tag;
                                    inherited.DefaultValue = defaultValue;
                                    inherited.Name = cutout;
                                    if (!string.IsNullOrEmpty(summary))
                                        inherited.Summary = summary;
                                    inherited.Export = new ExportInfo();
                                    newNode.Tag = inherited;
                                } else {
                                    newNode.Tag = new ElementInfo {
                                        Name = cutout,
                                        Attributes = attributes,
                                        DefaultValue = defaultValue,
                                        Extends = extends,
                                        Modifiers = modifiers.Trim(),
                                        Summary = summary,
                                        Type = type,
                                        Vis = vis,
                                        Kind = kind,
                                        Export = new ExportInfo()
                                    };
                                }
                            }
                            if (closing && node.Parent != null)
                                node = node.Parent;
                            summary = string.Empty;
                        }
                        break;
                    case '(':
                    case '[':
                        if (!inRemovableBlock && !commentLine && !preprocessorSkip)
                            ++parenthesis;
                        break;
                    case ')':
                    case ']':
                        if (lastEquals != i - 1 && !inRemovableBlock && !commentLine && !preprocessorSkip)
                            --parenthesis;
                        inTemplate = false;
                        break;
                    case '<':
                        // could be a simple "smaller than", but cancelled at all possibilities that break this assumption
                        inTemplate = true;
                        break;
                    case '?':
                    case '>':
                        inTemplate = false;
                        break;
                    case '/':
                        if (!preprocessorSkip) {
                            if (!commentLine) {
                                if (lastSlash == i - 1)
                                    commentLine = true;
                                lastSlash = i;
                            } else if (commentLine && !summaryLine) {
                                if (lastSlash == i - 1)
                                    summaryLine = true;
                                lastSlash = i;
                            }
                        }
                        break;
                    case '=':
                        lastEquals = i;
                        break;
                    case '#':
                        commentLine = preprocessorLine = true;
                        break;
                    case '\n':
                        if (preprocessorLine) {
                            string line = code.Substring(lastEnding, i - lastEnding).Trim();
                            if (line.StartsWith(_define)) {
                                string defined = line.Substring(line.IndexOf(' ')).TrimStart();
                                Array.Resize(ref defineConstants, ++defineCount);
                                defineConstants[defineCount - 1] = defined;
                            } else if (line.StartsWith(_if) || line.StartsWith(_elif)) {
                                int commentPos;
                                if ((commentPos = line.IndexOf("//")) != -1)
                                    line = line.Substring(0, commentPos).TrimEnd();
                                preprocessorSkip =
                                    !ParseSimpleIf(line.Substring(line.IndexOf(' ')).TrimStart(), defineConstants);
                            } else if (line.StartsWith(_endif))
                                preprocessorSkip = false;
                            preprocessorLine = false;
                        }
                        if (commentLine || preprocessorSkip) {
                            commentLine = false;
                            lastEnding = i + 1;
                        }
                        if (summaryLine) {
                            summary += code.Substring(lastSlash + 1, i - lastSlash - 1).Trim() + '\n';
                            summaryLine = false;
                        }
                        ++loc;
                        break;
                    default:
                        break;
                }
            }
            return loc;
        }

        const string splitRegex = @"\s+", _commentBlockStart = "/*", _commentBlockEnd = "*/", _indexStart = "[", _indexEnd = "]";
        const string _public = "public", _internal = "internal", _protected = "protected", _private = "private";
        const string _getter = "get", _setter = "set";
        internal const string _abstract = "abstract", _partial_ = "partial ", _static = "static", _using = "using";
        const string _class = "class", _enum = "enum", _interface = "interface", _namespace = "namespace", _struct = "struct";
        const string constructor = "Constructor", _delegate = "delegate";
        const string _true = "true", _false = "false", _and = "&&", _lambda = "=>";
        const string _define = "#define", _if = "#if", _elif = "#elif", _endif = "#endif";
    }
}