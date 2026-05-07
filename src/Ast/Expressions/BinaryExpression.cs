namespace TLPMinion.Ast.Expressions;

public sealed class BinaryExpression : Expression
{
    public BinaryExpression(Expression left, BinaryOperator op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public Expression Left { get; }

    public BinaryOperator Operator { get; }

    public Expression Right { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
