namespace DocSharp {
    public class GenericTestClass<T1, T2> { }
    public abstract class AbstractTestClass {
        GenericTestClass<string, int> GenericTestField = new GenericTestClass<string, int>();
        public abstract void AbstractTestFunction();
        public GenericTestClass<string, int> TestFunction() { return GenericTestField; }
    }
}