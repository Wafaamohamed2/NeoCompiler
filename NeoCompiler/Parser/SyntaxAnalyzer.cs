using NeoCompiler.Lexer;

namespace NeoCompiler.ParserModule
{
    public class SyntaxAnalyzer
    {
        private List<Token> _tokens;
        private int _pos;

        private Token Current => _tokens[_pos];
        private Token Peek => _pos + 1 < _tokens.Count ? _tokens[_pos + 1] : _tokens[^1];

        public SyntaxAnalyzer(List<Token> tokens) { _tokens = tokens; _pos = 0; }

        private Token Consume(TokenType type)
        {
            if (Current.Type != type)
                throw new CompilerException($"Expected '{type}' but found '{Current.Value}'", Current.Line);
            return _tokens[_pos++];
        }

        private bool Check(TokenType type) => Current.Type == type;

        private bool IsType() => Current.Type is TokenType.INT or TokenType.FLOAT_TYPE
                                              or TokenType.STRING_TYPE or TokenType.BOOL_TYPE
                                              or TokenType.ID;

        // ─── Program ───────────────────────────────────────────
        public ProgramNode Parse()
        {
            var program = new ProgramNode();
            while (!Check(TokenType.EOF))
                program.Statements.Add(ParseStatement());
            return program;
        }

        // ─── Statement ─────────────────────────────────────────
        private ASTNode ParseStatement()
        {
            if (Check(TokenType.CLASS)) return ParseClassDecl();
            if (Check(TokenType.FUNC)) return ParseFuncDecl();
            if (Check(TokenType.IF)) return ParseIf();
            if (Check(TokenType.WHILE)) return ParseWhile();
            if (Check(TokenType.FOR)) return ParseFor();
            if (Check(TokenType.RETURN)) return ParseReturn();
            if (Check(TokenType.PRINT)) return ParsePrint();
            if (IsType()) return ParseVarDecl();

            if (Check(TokenType.ID) && Peek.Type == TokenType.ASSIGN)
                return ParseAssign();

            if (Check(TokenType.ID) && Peek.Type == TokenType.OPAREN)
            {
                var call = ParseFuncCall();
                Consume(TokenType.SEMICOLON);
                return call;
            }

            throw new CompilerException($"Unexpected token '{Current.Value}'", Current.Line);
        }

        // ─── VarDecl ───────────────────────────────────────────
        private VarDeclNode ParseVarDecl()
        {
            string type = Current.Value; _pos++;
            string name = Consume(TokenType.ID).Value;
            ASTNode? value = null;
            if (Check(TokenType.ASSIGN)) { _pos++; value = ParseExpr(); }
            Consume(TokenType.SEMICOLON);
            return new VarDeclNode(type, name, value);
        }

        // ─── Assign ────────────────────────────────────────────
        private AssignNode ParseAssign()
        {
            string name = Consume(TokenType.ID).Value;
            Consume(TokenType.ASSIGN);
            var value = ParseExpr();
            Consume(TokenType.SEMICOLON);
            return new AssignNode(name, value);
        }

        // ─── If ────────────────────────────────────────────────
        private IfNode ParseIf()
        {
            Consume(TokenType.IF);
            Consume(TokenType.OPAREN);
            var condition = ParseExpr();
            Consume(TokenType.CPAREN);
            var thenBody = ParseBlock();
            List<ASTNode>? elseBody = null;
            if (Check(TokenType.ELSE)) { _pos++; elseBody = ParseBlock(); }
            return new IfNode(condition, thenBody, elseBody);
        }

        // ─── While ─────────────────────────────────────────────
        private WhileNode ParseWhile()
        {
            Consume(TokenType.WHILE);
            Consume(TokenType.OPAREN);
            var condition = ParseExpr();
            Consume(TokenType.CPAREN);
            return new WhileNode(condition, ParseBlock());
        }

        // ─── For ───────────────────────────────────────────────
        private ForNode ParseFor()
        {
            Consume(TokenType.FOR);
            Consume(TokenType.OPAREN);
            var init = ParseVarDecl();
            var condition = ParseExpr();
            Consume(TokenType.SEMICOLON);
            string stepName = Consume(TokenType.ID).Value;
            Consume(TokenType.ASSIGN);
            var stepVal = ParseExpr();
            var step = new AssignNode(stepName, stepVal);
            Consume(TokenType.CPAREN);
            return new ForNode(init, condition, step, ParseBlock());
        }

        // ─── FuncDecl ──────────────────────────────────────────
        private FuncDeclNode ParseFuncDecl()
        {
            Consume(TokenType.FUNC);
            string returnType = Current.Value; _pos++;
            string name = Consume(TokenType.ID).Value;
            Consume(TokenType.OPAREN);
            var parms = new List<(string, string)>();
            while (!Check(TokenType.CPAREN))
            {
                string pType = Current.Value; _pos++;
                string pName = Consume(TokenType.ID).Value;
                parms.Add((pType, pName));
                if (Check(TokenType.COMMA)) _pos++;
            }
            Consume(TokenType.CPAREN);
            return new FuncDeclNode(returnType, name, parms, ParseBlock());
        }

        // ─── Return ────────────────────────────────────────────
        private ReturnNode ParseReturn()
        {
            Consume(TokenType.RETURN);
            var val = ParseExpr();
            Consume(TokenType.SEMICOLON);
            return new ReturnNode(val);
        }

        // ─── Print ─────────────────────────────────────────────
        private PrintNode ParsePrint()
        {
            Consume(TokenType.PRINT);
            Consume(TokenType.OPAREN);
            var val = ParseExpr();
            Consume(TokenType.CPAREN);
            Consume(TokenType.SEMICOLON);
            return new PrintNode(val);
        }

        // ─── ClassDecl ─────────────────────────────────────────
        private ClassDeclNode ParseClassDecl()
        {
            Consume(TokenType.CLASS);
            string name = Consume(TokenType.ID).Value;
            return new ClassDeclNode(name, ParseBlock());
        }

        // ─── Block { ... } ─────────────────────────────────────
        private List<ASTNode> ParseBlock()
        {
            Consume(TokenType.OBRACE);
            var stmts = new List<ASTNode>();
            while (!Check(TokenType.CBRACE) && !Check(TokenType.EOF))
                stmts.Add(ParseStatement());
            Consume(TokenType.CBRACE);
            return stmts;
        }

        // ─── Expressions ───────────────────────────────────────
        private ASTNode ParseExpr() => ParseOr();

        private ASTNode ParseOr()
        {
            var left = ParseAnd();
            while (Check(TokenType.OR))
            { _pos++; left = new BinaryOpNode(left, "||", ParseAnd()); }
            return left;
        }

        private ASTNode ParseAnd()
        {
            var left = ParseEquality();
            while (Check(TokenType.AND))
            { _pos++; left = new BinaryOpNode(left, "&&", ParseEquality()); }
            return left;
        }

        private ASTNode ParseEquality()
        {
            var left = ParseComparison();
            while (Check(TokenType.EQUAL) || Check(TokenType.NOT_EQUAL))
            { string op = Current.Value; _pos++; left = new BinaryOpNode(left, op, ParseComparison()); }
            return left;
        }

        private ASTNode ParseComparison()
        {
            var left = ParseAddSub();
            while (Check(TokenType.LESS) || Check(TokenType.GREATER) ||
                   Check(TokenType.LESS_EQ) || Check(TokenType.GREATER_EQ))
            { string op = Current.Value; _pos++; left = new BinaryOpNode(left, op, ParseAddSub()); }
            return left;
        }

        private ASTNode ParseAddSub()
        {
            var left = ParseMulDiv();
            while (Check(TokenType.PLUS) || Check(TokenType.MINUS))
            { string op = Current.Value; _pos++; left = new BinaryOpNode(left, op, ParseMulDiv()); }
            return left;
        }

        private ASTNode ParseMulDiv()
        {
            var left = ParseUnary();
            while (Check(TokenType.STAR) || Check(TokenType.SLASH))
            { string op = Current.Value; _pos++; left = new BinaryOpNode(left, op, ParseUnary()); }
            return left;
        }

        private ASTNode ParseUnary()
        {
            if (Check(TokenType.NOT)) { _pos++; return new UnaryOpNode("!", ParseUnary()); }
            if (Check(TokenType.MINUS)) { _pos++; return new UnaryOpNode("-", ParseUnary()); }
            return ParsePrimary();
        }

        private ASTNode ParsePrimary()
        {
            if (Check(TokenType.INTEGER)) return new IntLiteralNode(int.Parse(_tokens[_pos++].Value));
            if (Check(TokenType.FLOAT)) return new FloatLiteralNode(float.Parse(_tokens[_pos++].Value));
            if (Check(TokenType.STRING)) return new StringLiteralNode(_tokens[_pos++].Value);
            if (Check(TokenType.BOOL)) return new BoolLiteralNode(_tokens[_pos++].Value == "true");

            if (Check(TokenType.NEW))
            {
                _pos++;
                string className = Consume(TokenType.ID).Value;
                Consume(TokenType.OPAREN);
                var args = ParseArgs();
                Consume(TokenType.CPAREN);
                return new NewObjectNode(className, args);
            }

            if (Check(TokenType.ID) && Peek.Type == TokenType.OPAREN)
                return ParseFuncCall();

            if (Check(TokenType.ID))
                return new IdentifierNode(_tokens[_pos++].Value);

            if (Check(TokenType.OPAREN))
            {
                _pos++;
                var expr = ParseExpr();
                Consume(TokenType.CPAREN);
                return expr;
            }

            throw new CompilerException($"Unexpected token '{Current.Value}'", Current.Line);
        }

        // ─── FuncCall ──────────────────────────────────────────
        private FuncCallNode ParseFuncCall()
        {
            string name = Consume(TokenType.ID).Value;
            Consume(TokenType.OPAREN);
            var args = ParseArgs();
            Consume(TokenType.CPAREN);
            return new FuncCallNode(name, args);
        }

        private List<ASTNode> ParseArgs()
        {
            var args = new List<ASTNode>();
            while (!Check(TokenType.CPAREN))
            {
                args.Add(ParseExpr());
                if (Check(TokenType.COMMA)) _pos++;
            }
            return args;
        }
    }
}
