using System.Text;

namespace NeoCompiler.Lexer
{
    public enum LexerState
    {
        Start,
        Identifier,
        Number,
        Float,
        String,
        Operator,
        SingleLineComment,
        Done
    }

    public class LexicalAnalyzer
    {
        private readonly string _source;
        private int _pos;
        private int _line;

        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            { "int",    TokenType.INT },
            { "float",  TokenType.FLOAT_TYPE },
            { "string", TokenType.STRING_TYPE },
            { "bool",   TokenType.BOOL_TYPE },
            { "if",     TokenType.IF },
            { "else",   TokenType.ELSE },
            { "while",  TokenType.WHILE },
            { "for",    TokenType.FOR },
            { "func",   TokenType.FUNC },
            { "class",  TokenType.CLASS },
            { "new",    TokenType.NEW },
            { "return", TokenType.RETURN },
            { "print",  TokenType.PRINT },
            { "true",   TokenType.BOOL },
            { "false",  TokenType.BOOL }
        };

        public LexicalAnalyzer(string source)
        {
            _source = source;
            _pos = 0;
            _line = 1;
        }

        private char Current => _pos < _source.Length ? _source[_pos] : '\0';
        private void Advance() { if (Current == '\n') _line++; _pos++; }
        private char Peek => _pos + 1 < _source.Length ? _source[_pos + 1] : '\0';

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (_pos < _source.Length)
            {
                if (char.IsWhiteSpace(Current))
                {
                    Advance();
                    continue;
                }

                var token = GetNextToken();
                if (token != null) tokens.Add(token);
            }

            tokens.Add(new Token(TokenType.EOF, "", _line));
            return tokens;
        }

        private Token? GetNextToken()
        {
            var state = LexerState.Start;
            var buffer = new StringBuilder();
            int startLine = _line;

            while (_pos <= _source.Length)
            {
                char c = Current;

                switch (state)
                {
                    case LexerState.Start:
                        if (c == '\0') return null;
                        if (char.IsWhiteSpace(c)) { Advance(); return null; }

                        if (char.IsLetter(c) || c == '_')
                        {
                            state = LexerState.Identifier;
                            buffer.Append(c);
                            Advance();
                        }
                        else if (char.IsDigit(c))
                        {
                            state = LexerState.Number;
                            buffer.Append(c);
                            Advance();
                        }
                        else if (c == '"')
                        {
                            state = LexerState.String;
                            Advance(); 
                        }
                        else if (c == '/' && Peek == '/')
                        {
                            state = LexerState.SingleLineComment;
                            Advance(); Advance();
                        }
                        else
                        {
                            return ReadOperatorOrSymbol();
                        }
                        break;

                    case LexerState.Identifier:
                        if (char.IsLetterOrDigit(c) || c == '_')
                        {
                            buffer.Append(c);
                            Advance();
                        }
                        else
                        {
                            string word = buffer.ToString();
                            TokenType type = Keywords.ContainsKey(word) ? Keywords[word] : TokenType.ID;
                            return new Token(type, word, startLine);
                        }
                        break;

                    case LexerState.Number:
                        if (char.IsDigit(c))
                        {
                            buffer.Append(c);
                            Advance();
                        }
                        else if (c == '.')
                        {
                            state = LexerState.Float;
                            buffer.Append(c);
                            Advance();
                        }
                        else
                        {
                            return new Token(TokenType.INTEGER, buffer.ToString(), startLine);
                        }
                        break;

                    case LexerState.Float:
                        if (char.IsDigit(c))
                        {
                            buffer.Append(c);
                            Advance();
                        }
                        else
                        {
                            return new Token(TokenType.FLOAT, buffer.ToString(), startLine);
                        }
                        break;

                    case LexerState.String:
                        if (c == '"')
                        {
                            Advance(); 
                            return new Token(TokenType.STRING, buffer.ToString(), startLine);
                        }
                        else if (c == '\0')
                        {
                            throw new CompilerException($"Unterminated string literal.", startLine, "Lexical Error");
                        }
                        else
                        {
                            buffer.Append(c);
                            Advance();
                        }
                        break;

                    case LexerState.SingleLineComment:
                        if (c == '\n' || c == '\0')
                        {
                            state = LexerState.Start;
                            return null;
                        }
                        Advance();
                        break;
                }
            }
            return null;
        }

        private Token ReadOperatorOrSymbol()
        {
            char c = Current;
            int line = _line;
            Advance();

            switch (c)
            {
                case '+': return new Token(TokenType.PLUS, "+", line);
                case '-': return new Token(TokenType.MINUS, "-", line);
                case '*': return new Token(TokenType.STAR, "*", line);
                case '/': return new Token(TokenType.SLASH, "/", line);
                case '(': return new Token(TokenType.OPAREN, "(", line);
                case ')': return new Token(TokenType.CPAREN, ")", line);
                case '{': return new Token(TokenType.OBRACE, "{", line);
                case '}': return new Token(TokenType.CBRACE, "}", line);
                case ',': return new Token(TokenType.COMMA, ",", line);
                case ';': return new Token(TokenType.SEMICOLON, ";", line);
                case '.': return new Token(TokenType.DOT, ".", line);

                case '!':
                    if (Current == '=') { Advance(); return new Token(TokenType.NOT_EQUAL, "!=", line); }
                    return new Token(TokenType.NOT, "!", line);

                case '=':
                    if (Current == '=') { Advance(); return new Token(TokenType.EQUAL, "==", line); }
                    return new Token(TokenType.ASSIGN, "=", line);

                case '<':
                    if (Current == '=') { Advance(); return new Token(TokenType.LESS_EQ, "<=", line); }
                    return new Token(TokenType.LESS, "<", line);

                case '>':
                    if (Current == '=') { Advance(); return new Token(TokenType.GREATER_EQ, ">=", line); }
                    return new Token(TokenType.GREATER, ">", line);

                case '&':
                    if (Current == '&') { Advance(); return new Token(TokenType.AND, "&&", line); }
                    throw new CompilerException($"Expected '&' after '&' to form '&&'.", line, "Lexical Error");

                case '|':
                    if (Current == '|') { Advance(); return new Token(TokenType.OR, "||", line); }
                    throw new CompilerException($"Expected '|' after '|' to form '||'.", line, "Lexical Error");

                default:
                    throw new CompilerException($"Unknown character '{c}' was found.", line, "Lexical Error");
            }
        }
    }
}
