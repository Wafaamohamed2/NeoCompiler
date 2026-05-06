namespace NeoCompiler.Lexer
{
    public enum TokenType
    {
        // Types
        INT, FLOAT_TYPE, STRING_TYPE, BOOL_TYPE, STRING_LITERAL, CHAR_LITERAL,

        // Keywords
        IF, ELSE, WHILE, FOR, FUNC, CLASS, NEW, RETURN, PRINT,

        // Literals
        INTEGER,      
        FLOAT,        
        STRING,       
        BOOL,

        // Identifier
        ID,

        // Operators
        PLUS,         
        MINUS,        
        STAR,         
        SLASH,       
        ASSIGN,       
        EQUAL,        
        NOT_EQUAL,    
        LESS,         
        GREATER,      
        LESS_EQ,      
        GREATER_EQ,   
        AND,         
        OR,           
        NOT,          


        // Symbols
        OPAREN,       
        CPAREN,       
        OBRACE,       
        CBRACE,       
        COMMA,        
        SEMICOLON,   
        DOT,          

        EOF           //End of file   
    }
}
