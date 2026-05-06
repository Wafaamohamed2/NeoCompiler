### NeoCompiler
A custom, hand-written compiler built with C# and .NET 8.0. 
This project implements a full lexical and syntax analysis pipeline, featuring a DFA-based Lexer and a Recursive Descent Parser to generate and visualize an Abstract Syntax Tree (AST).

### Features
  - Lexical Analysis (DFA): High-performance tokenization using a Deterministic Finite Automata approach.
  - Syntax Analysis (Top-Down): A robust Recursive Descent Parser that validates code according to custom grammar.
  - AST Visualization: Real-time tree view representation of the parsed code structure.
  - Interactive UI: Built with WinForms for a seamless coding and debugging experience.
  - Error Handling: Detailed compiler error messages with line numbers and error types.

### Architecture
The compiler is divided into several modular components:
   1. LexicalAnalyzer (Lexer): Scans the source code and breaks it into tokens (Keywords, Identifiers, Operators, Literals).
   2. SyntaxAnalyzer (Parser): Validates the sequence of tokens and builds the logical structure of the program.

## Supported Grammar
NeoCompiler supports a custom language with features like:
   - Variable Declarations: int x = 5;
   - Control Flow: if, else, while, for loops.
   - Functions: func int add(int a, int b) { ... }
   - Object-Oriented: class Person { ... } and object instantiation using new.
   - Built-in Functions: print(value);

   
