namespace NeoCompiler
{
    public class CompilerException : Exception
    {
        public int Line { get; }
        public string ErrorType { get; }

        public CompilerException(string message, int line, string errorType = "Syntax Error") 
            : base(message)
        {
            Line = line;
            ErrorType = errorType;
        }

        public override string ToString()
        {
            return $"{ErrorType} at line {Line}: {Message}";
        }
    }
}
