using System.Text;
using NeoCompiler.ParserModule;

namespace NeoCompiler
{
    public partial class MainForm : Form
    {
        private TextBox codeTextBox;
        private Button compileButton;
        private TabControl tabControl;
        private TabPage tabTokens;
        private TabPage tabOutput;
        private DataGridView tokensGrid;
        private RichTextBox outputTextBox;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.codeTextBox = new TextBox();
            this.compileButton = new Button();
            this.tabControl = new TabControl();
            this.tabTokens = new TabPage();
            this.tabOutput = new TabPage();
            this.tokensGrid = new DataGridView();
            this.outputTextBox = new RichTextBox();
            
            this.tabControl.SuspendLayout();
            this.tabTokens.SuspendLayout();
            this.tabOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tokensGrid)).BeginInit();
            this.SuspendLayout();

            // codeTextBox
            this.codeTextBox.AcceptsReturn = true;
            this.codeTextBox.AcceptsTab = true;
            this.codeTextBox.Dock = DockStyle.Top;
            this.codeTextBox.Font = new Font("Consolas", 12F, FontStyle.Regular);
            this.codeTextBox.Location = new Point(0, 0);
            this.codeTextBox.Multiline = true;
            this.codeTextBox.Name = "codeTextBox";
            this.codeTextBox.Size = new Size(1000, 300);
            this.codeTextBox.TabIndex = 0;
            this.codeTextBox.Text = "class person{\r\n   int x = 3+3;\r\n   print(x);\r\n}\r\n\r\nclass male{\r\n   person p = new person();\r\n}";

            // compileButton
            this.compileButton.Dock = DockStyle.Top;
            this.compileButton.Location = new Point(0, 300);
            this.compileButton.Name = "compileButton";
            this.compileButton.Size = new Size(1000, 45);
            this.compileButton.TabIndex = 1;
            this.compileButton.Text = "Compile && Run";
            this.compileButton.BackColor = Color.ForestGreen;
            this.compileButton.ForeColor = Color.White;
            this.compileButton.FlatStyle = FlatStyle.Flat;
            this.compileButton.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.compileButton.Click += new EventHandler(this.CompileButton_Click);

            // tabControl
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 345);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(1000, 355);
            this.tabControl.TabIndex = 2;
            this.tabControl.Controls.Add(this.tabTokens);
            this.tabControl.Controls.Add(this.tabOutput);

            // tabTokens
            this.tabTokens.Controls.Add(this.tokensGrid);
            this.tabTokens.Location = new Point(4, 24);
            this.tabTokens.Name = "tabTokens";
            this.tabTokens.Padding = new Padding(3);
            this.tabTokens.Size = new Size(992, 327);
            this.tabTokens.TabIndex = 0;
            this.tabTokens.Text = "Lexical Analysis (Tokens)";
            this.tabTokens.UseVisualStyleBackColor = true;

            // tokensGrid
            this.tokensGrid.AllowUserToAddRows = false;
            this.tokensGrid.AllowUserToDeleteRows = false;
            this.tokensGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.tokensGrid.BackgroundColor = Color.White;
            this.tokensGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tokensGrid.Dock = DockStyle.Fill;
            this.tokensGrid.Location = new Point(3, 3);
            this.tokensGrid.Name = "tokensGrid";
            this.tokensGrid.ReadOnly = true;
            this.tokensGrid.RowHeadersVisible = false;
            this.tokensGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.tokensGrid.Size = new Size(986, 321);
            this.tokensGrid.TabIndex = 0;
            this.tokensGrid.Columns.Add("Type", "Token Type");
            this.tokensGrid.Columns.Add("Lexeme", "Lexeme");
            this.tokensGrid.Columns.Add("Line", "Line");

            // tabOutput
            this.tabOutput.Controls.Add(this.outputTextBox);
            this.tabOutput.Location = new Point(4, 24);
            this.tabOutput.Name = "tabOutput";
            this.tabOutput.Padding = new Padding(3);
            this.tabOutput.Size = new Size(992, 327);
            this.tabOutput.TabIndex = 1;
            this.tabOutput.Text = "Parser Output (AST)";
            this.tabOutput.UseVisualStyleBackColor = true;

            // outputTextBox
            this.outputTextBox.Dock = DockStyle.Fill;
            this.outputTextBox.BackColor = Color.Black;
            this.outputTextBox.ForeColor = Color.White;
            this.outputTextBox.Font = new Font("Consolas", 11F, FontStyle.Regular);
            this.outputTextBox.Location = new Point(3, 3);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.Size = new Size(986, 321);
            this.outputTextBox.TabIndex = 0;
            this.outputTextBox.Text = "";

            // MainForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.compileButton);
            this.Controls.Add(this.codeTextBox);
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "NeoCompiler - DFA & Recursive Descent Parser";
            
            this.tabControl.ResumeLayout(false);
            this.tabTokens.ResumeLayout(false);
            this.tabOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tokensGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CompileButton_Click(object? sender, EventArgs e)
        {
            outputTextBox.Clear();
            tokensGrid.Rows.Clear();
            string code = codeTextBox.Text;

            try
            {
                var lexer = new NeoCompiler.Lexer.LexicalAnalyzer(code);
                var tokens = lexer.Tokenize();

                foreach (var token in tokens)
                {
                    tokensGrid.Rows.Add(token.Type.ToString(), token.Value, token.Line.ToString());
                }

                outputTextBox.AppendText("--- Syntax Analysis (Top-Down Parser) ---\n");
                var parser = new SyntaxAnalyzer(tokens);
                var ast = parser.Parse();

                outputTextBox.AppendText($"Successfully parsed {ast.Statements.Count} statements.\n\n");
                outputTextBox.AppendText("--- Parse Tree ---\n");
                outputTextBox.AppendText(PrintAST(ast));

                outputTextBox.SelectionColor = Color.LightGreen;
                outputTextBox.AppendText("\nCompilation Successful!");
            }
            catch (CompilerException ex)
            {
                outputTextBox.SelectionColor = Color.Red;
                outputTextBox.AppendText($"\n[ERROR] {ex.ErrorType}: {ex.Message} at line {ex.Line}");
                MessageBox.Show(ex.Message, ex.ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                outputTextBox.SelectionColor = Color.Red;
                outputTextBox.AppendText($"\n[FATAL ERROR]: {ex.Message}");
            }
        }

        private string PrintAST(ProgramNode program)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Program");
            for (int i = 0; i < program.Statements.Count; i++)
            {
                bool isLast = i == program.Statements.Count - 1;
                PrintNode(program.Statements[i], sb, "", isLast);
            }
            return sb.ToString();
        }

        private void PrintNode(ASTNode node, StringBuilder sb, string indent, bool isLast)
        {
            if (node == null) return;

            string marker = isLast ? "└── " : "├── ";
            sb.Append(indent);
            sb.Append(marker);

            string nextIndent = indent + (isLast ? "    " : "│   ");

            if (node is VarDeclNode varDecl)
            {
                sb.AppendLine($"VarDecl: {varDecl.Type} {varDecl.Name}");
                if (varDecl.Value != null)
                {
                    PrintNode(varDecl.Value, sb, nextIndent, true);
                }
            }
            else if (node is AssignNode assign)
            {
                sb.AppendLine($"Assign: {assign.Name}");
                PrintNode(assign.Value, sb, nextIndent, true);
            }
            else if (node is IfNode ifNode)
            {
                sb.AppendLine("IfStatement");
                PrintNode(ifNode.Condition, sb, nextIndent, false);
                
                bool hasElse = ifNode.ElseBody != null && ifNode.ElseBody.Count > 0;
                
                sb.AppendLine(nextIndent + (hasElse ? "├── " : "└── ") + "Then");
                string thenIndent = nextIndent + (hasElse ? "│   " : "    ");
                for (int i = 0; i < ifNode.ThenBody.Count; i++)
                    PrintNode(ifNode.ThenBody[i], sb, thenIndent, i == ifNode.ThenBody.Count - 1);

                if (hasElse)
                {
                    sb.AppendLine(nextIndent + "└── Else");
                    string elseIndent = nextIndent + "    ";
                    for (int i = 0; i < ifNode.ElseBody.Count; i++)
                        PrintNode(ifNode.ElseBody[i], sb, elseIndent, i == ifNode.ElseBody.Count - 1);
                }
            }
            else if (node is WhileNode whileNode)
            {
                sb.AppendLine("WhileStatement");
                PrintNode(whileNode.Condition, sb, nextIndent, false);
                
                sb.AppendLine(nextIndent + "└── Body");
                string bodyIndent = nextIndent + "    ";
                for (int i = 0; i < whileNode.Body.Count; i++)
                    PrintNode(whileNode.Body[i], sb, bodyIndent, i == whileNode.Body.Count - 1);
            }
            else if (node is ForNode forNode)
            {
                sb.AppendLine("ForStatement");
                PrintNode(forNode.Init, sb, nextIndent, false);
                PrintNode(forNode.Condition, sb, nextIndent, false);
                PrintNode(forNode.Step, sb, nextIndent, false);
                
                sb.AppendLine(nextIndent + "└── Body");
                string bodyIndent = nextIndent + "    ";
                for (int i = 0; i < forNode.Body.Count; i++)
                    PrintNode(forNode.Body[i], sb, bodyIndent, i == forNode.Body.Count - 1);
            }
            else if (node is FuncDeclNode funcDecl)
            {
                sb.AppendLine($"FuncDecl: {funcDecl.ReturnType} {funcDecl.Name}");
                foreach (var p in funcDecl.Params)
                {
                    sb.AppendLine(nextIndent + $"├── Param: {p.Type} {p.Name}");
                }
                sb.AppendLine(nextIndent + "└── Body");
                string bodyIndent = nextIndent + "    ";
                for (int i = 0; i < funcDecl.Body.Count; i++)
                    PrintNode(funcDecl.Body[i], sb, bodyIndent, i == funcDecl.Body.Count - 1);
            }
            else if (node is ReturnNode returnNode)
            {
                sb.AppendLine("Return");
                PrintNode(returnNode.Value, sb, nextIndent, true);
            }
            else if (node is PrintNode printNode)
            {
                sb.AppendLine("Print");
                PrintNode(printNode.Value, sb, nextIndent, true);
            }
            else if (node is ClassDeclNode classDecl)
            {
                sb.AppendLine($"ClassDecl: {classDecl.Name}");
                sb.AppendLine(nextIndent + "└── Body");
                string bodyIndent = nextIndent + "    ";
                for (int i = 0; i < classDecl.Body.Count; i++)
                    PrintNode(classDecl.Body[i], sb, bodyIndent, i == classDecl.Body.Count - 1);
            }
            else if (node is FuncCallNode funcCall)
            {
                sb.AppendLine($"FuncCall: {funcCall.Name}");
                for (int i = 0; i < funcCall.Args.Count; i++)
                    PrintNode(funcCall.Args[i], sb, nextIndent, i == funcCall.Args.Count - 1);
            }
            else if (node is NewObjectNode newObj)
            {
                sb.AppendLine($"NewObject: {newObj.ClassName}");
                for (int i = 0; i < newObj.Args.Count; i++)
                    PrintNode(newObj.Args[i], sb, nextIndent, i == newObj.Args.Count - 1);
            }
            else if (node is BinaryOpNode binOp)
            {
                sb.AppendLine($"BinaryOp: {binOp.Op}");
                PrintNode(binOp.Left, sb, nextIndent, false);
                PrintNode(binOp.Right, sb, nextIndent, true);
            }
            else if (node is UnaryOpNode unaryOp)
            {
                sb.AppendLine($"UnaryOp: {unaryOp.Op}");
                PrintNode(unaryOp.Operand, sb, nextIndent, true);
            }
            else if (node is IntLiteralNode intLit)
            {
                sb.AppendLine($"IntLiteral: {intLit.Value}");
            }
            else if (node is FloatLiteralNode floatLit)
            {
                sb.AppendLine($"FloatLiteral: {floatLit.Value}");
            }
            else if (node is StringLiteralNode stringLit)
            {
                sb.AppendLine($"StringLiteral: \"{stringLit.Value}\"");
            }
            else if (node is BoolLiteralNode boolLit)
            {
                sb.AppendLine($"BoolLiteral: {(boolLit.Value ? "true" : "false")}");
            }
            else if (node is IdentifierNode idNode)
            {
                sb.AppendLine($"Identifier: {idNode.Name}");
            }
            else
            {
                sb.AppendLine(node.GetType().Name);
            }
        }
    }
}
