namespace TLPMinion.Ast.Expressions;

public sealed class LiteralExpression : Expression
{
    public LiteralExpression(string typeName, string lexeme)
    {
        TypeName = typeName;
        Lexeme = lexeme;
    }

    public string TypeName { get; }

    public string Lexeme { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
