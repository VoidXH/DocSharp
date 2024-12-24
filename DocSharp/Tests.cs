#define TEST

// This namespace is used for testing the documentation of behavior not present in the DocSharp code.
namespace DocSharp.Tests {
    /// <summary>
    /// Test generics.
    /// </summary>
    /// <typeparam name="T1">Type parameter summary</typeparam>
    /// <typeparam name="T2">Other type parameter summary</typeparam>
    public class GenericTestClass<T1, T2> { }

    /// <summary>
    /// Test abstract classes.
    /// </summary>
    public abstract class AbstractTestClass {
#if RELEASE  &&  (TEST || UNDEFINED) && true // Tests the preprocessor instruction parser
        public class EmbeddedClass { public const string Hello = "Hello, \"World\"!"; }
#endif
        readonly GenericTestClass<string, int> GenericTestField = new();
        /// <summary>
        /// Test abstract functions.
        /// </summary>
        public abstract void AbstractTestFunction();

        /// <summary>
        /// Test generic functions.
        /// </summary>
        /// <param name="x">Parameter summary</param>
        public GenericTestClass<string, int> TestFunction(int x) { return GenericTestField; }
    }
}