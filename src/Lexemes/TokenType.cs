namespace TLPMinion.Lexemes;

/// <summary>Вид лексемы (<c>Multiply</c>, <c>OpenParenthesis</c> и т.д.).</summary>
public enum TokenType
{
    EndOfFile,

    Error,

    Identifier,
    IntLiteral,
    FloatLiteral,
    StringLiteral,
    Const,
    Let,
    Var,
    Print,
    Input,
    Length,
    Substring,

    TypeInt,
    TypeFloat,
    TypeString,
    TypeVoid,

    Semicolon,
    Comma,
    Colon,
    OpenParenthesis,
    CloseParenthesis,
    OpenBrace,
    CloseBrace,

    Assign,
    Plus,
    Minus,
    Multiply,
    Divide,
    Percent,
    Power,
}