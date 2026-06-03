namespace TLPMinion.Ast.Expressions;

public sealed class ConditionalExpression : Expression
{
    public ConditionalExpression(Expression condition, Expression whenTrue, Expression whenFalse)
    {
        Condition = condition;
        WhenTrue = whenTrue;
        WhenFalse = whenFalse;
    }

    public Expression Condition { get; }

    public Expression WhenTrue { get; }

    public Expression WhenFalse { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
