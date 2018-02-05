namespace DocSharp {
    public class GenericTestClass<T1, T2> { }
    public abstract class AbstractTestClass {
#if DEBUG
        public class EmbeddedClass { public const string Hello = "Hello, \"World\"!"; }
#endif
        GenericTestClass<string, int> GenericTestField = new GenericTestClass<string, int>();
        public abstract void AbstractTestFunction();
        public GenericTestClass<string, int> TestFunction() { return GenericTestField; }
    }
}