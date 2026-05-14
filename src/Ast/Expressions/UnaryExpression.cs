namespace TLPMinion.Ast.Expressions;

public sealed class UnaryExpression : Expression
{
    public UnaryExpression(UnaryOperator op, Expression operand)
    {
        Operator = op;
        Operand = operand;
    }

    public UnaryOperator Operator { get; }

    public Expression Operand { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
