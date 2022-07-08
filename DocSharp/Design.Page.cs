namespace DocSharp {
    // HTML design of the exporter.
    static partial class Design {
        // Replace markers for the exporter.
        const string
            titleMarker = "<!--title-->",
            menuMarker = "<!--menu-->",
            cssMarker = "<!--css-->",
            elementMarker = "<!--elem-->",
            linkMarker = "<!--link-->",
            indentMarker = "<!--indent-->",
            subelementMarker = "<!--sub-->",
            contentMarker = "<!--content-->";

        /// <summary>
        /// Path to the stylesheet.
        /// </summary>
        const string stylesheet = "style.css";

        // Class names in the stylesheet.
        const string
            mainTableClass = "mt",
            menuTdClass = "t1",
            firstColumnClass = "t2",
            evenRowClass = "sr";

        // Class name use helpers.
        const string
            firstColumnClassTag = " class=\"" + firstColumnClass + "\"",
            evenRowClassTag = " class=\"" + evenRowClass + "\"";

        /// <summary>
        /// Parent menu element.
        /// </summary>
        const string menuElement = "<h2>" + indentMarker + "<a href=\"" + linkMarker + "\">" + elementMarker + "</a></h2>";
        /// <summary>
        /// Menu element on the same level as the opened module.
        /// </summary>
        const string menuSubelement = "<h3>" + indentMarker + "<a href=\"" + linkMarker + "\">" + elementMarker + "</a></h3>";

        /// <summary>
        /// Boilerplate of a page.
        /// </summary>
        const string baseBuild =
            "<html>" +
                "<head>" +
                    "<title>" + titleMarker + "</title><link rel=\"stylesheet\" href=\"" + cssMarker + "\" type=\"text/css\">" +
                "</head>" +
                "<body>" +
                    "<table class=\"" + mainTableClass + "\">" +
                        "<tr>" +
                            "<td class=\"" + menuTdClass + "\">" + menuMarker + "</td>" +
                            "<td>" + contentMarker + "</td>" +
                        "</tr>" +
                    "</table>" +
                "</body>" +
            "</html>";

        /// <summary>
        /// List row (e.g. for parameters).
        /// </summary>
        const string contentEntry =
            "<tr" + subelementMarker + ">" +
                "<td" + cssMarker + ">" + elementMarker + "</td>" +
                "<td>" + contentMarker + "</td>" +
            "</tr>";

        /// <summary>
        /// Stylesheet of the documentation.
        /// </summary>
        const string style =
"." + mainTableClass + @" { height: 100%; }
." + evenRowClass + @" { background-color: #EEEEEE; }
." + menuTdClass + @" { width: 250px; }
." + firstColumnClass + @" { width: 350px; }
a:link { color: red; text-decoration: none; }
a:visited { color: red; text-decoration: none; }
a:active { color: red; text-decoration: underline; }
a:hover { color: red; text-decoration: underline; }
h1 {
  font-size: 24px;
  margin: 0;
  margin-bottom: 6px;
}
h2 {
  font-size: 16px;
  margin: 0;
}
h3 {
  font-size: 14px;
  margin: 0;
}
html, body {
  font-family: Verdana;
  height: 100%;
  margin: 0;
}
table {
  border: none;
  padding-bottom: 8px;
  width: 100%;
}
table tr td {
  text-align: left;
  vertical-align: top;
}";
    }
}