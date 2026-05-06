namespace NeoCompiler.ParserModule
{
    
    public abstract class ASTNode { }

    // Program
    public class ProgramNode : ASTNode
    {
        public List<ASTNode> Statements = new();
    }

    // int x = 5;
    public class VarDeclNode : ASTNode
    {
        public string Type;
        public string Name;
        public ASTNode? Value;
        public VarDeclNode(string type, string name, ASTNode? value)
        { Type = type; Name = name; Value = value; }
    }

    // x = 5;
    public class AssignNode : ASTNode
    {
        public string Name;
        public ASTNode Value;
        public AssignNode(string name, ASTNode value)
        { Name = name; Value = value; }
    }

    // if / else
    public class IfNode : ASTNode
    {
        public ASTNode Condition;
        public List<ASTNode> ThenBody;
        public List<ASTNode>? ElseBody;
        public IfNode(ASTNode condition, List<ASTNode> thenBody, List<ASTNode>? elseBody)
        { Condition = condition; ThenBody = thenBody; ElseBody = elseBody; }
    }

    // while
    public class WhileNode : ASTNode
    {
        public ASTNode Condition;
        public List<ASTNode> Body;
        public WhileNode(ASTNode condition, List<ASTNode> body)
        { Condition = condition; Body = body; }
    }

    // for
    public class ForNode : ASTNode
    {
        public ASTNode Init;
        public ASTNode Condition;
        public ASTNode Step;
        public List<ASTNode> Body;
        public ForNode(ASTNode init, ASTNode condition, ASTNode step, List<ASTNode> body)
        { Init = init; Condition = condition; Step = step; Body = body; }
    }

    // func 
    public class FuncDeclNode : ASTNode
    {
        public string ReturnType;
        public string Name;
        public List<(string Type, string Name)> Params;
        public List<ASTNode> Body;
        public FuncDeclNode(string returnType, string name, List<(string, string)> parms, List<ASTNode> body)
        { ReturnType = returnType; Name = name; Params = parms; Body = body; }
    }

    // return
    public class ReturnNode : ASTNode
    {
        public ASTNode Value;
        public ReturnNode(ASTNode value) { Value = value; }
    }

    // print
    public class PrintNode : ASTNode
    {
        public ASTNode Value;
        public PrintNode(ASTNode value) { Value = value; }
    }

    // class
    public class ClassDeclNode : ASTNode
    {
        public string Name;
        public List<ASTNode> Body;
        public ClassDeclNode(string name, List<ASTNode> body)
        { Name = name; Body = body; }
    }

    // add
    public class FuncCallNode : ASTNode
    {
        public string Name;
        public List<ASTNode> Args;
        public FuncCallNode(string name, List<ASTNode> args)
        { Name = name; Args = args; }
    }

    // new
    public class NewObjectNode : ASTNode
    {
        public string ClassName;
        public List<ASTNode> Args;
        public NewObjectNode(string className, List<ASTNode> args)
        { ClassName = className; Args = args; }
    }

    // x + 5  or  x > 3
    public class BinaryOpNode : ASTNode
    {
        public ASTNode Left;
        public string Op;
        public ASTNode Right;
        public BinaryOpNode(ASTNode left, string op, ASTNode right)
        { Left = left; Op = op; Right = right; }
    }

    // !x
    public class UnaryOpNode : ASTNode
    {
        public string Op;
        public ASTNode Operand;
        public UnaryOpNode(string op, ASTNode operand)
        { Op = op; Operand = operand; }
    }

    // Literals
    public class IntLiteralNode : ASTNode { public int Value; public IntLiteralNode(int v) { Value = v; } }
    public class FloatLiteralNode : ASTNode { public float Value; public FloatLiteralNode(float v) { Value = v; } }
    public class StringLiteralNode : ASTNode { public string Value; public StringLiteralNode(string v) { Value = v; } }
    public class BoolLiteralNode : ASTNode { public bool Value; public BoolLiteralNode(bool v) { Value = v; } }

    // identifier
    public class IdentifierNode : ASTNode
    {
        public string Name;
        public IdentifierNode(string name) { Name = name; }
    }
}
