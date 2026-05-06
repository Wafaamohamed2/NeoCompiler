namespace NeoCompiler.Lexer
{
    public class Token
    {
        public TokenType Type;
        public string Value;
        public int Line;

        public Token(TokenType type, string value, int line)
        {
            Type = type;
            Value = value;
            Line = line;
        }

        public override string ToString()
        {
            return $"| {Type,-15} | {Value,-20} | {Line,-6} |";
        }
    }
}
