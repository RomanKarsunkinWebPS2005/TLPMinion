namespace TLPMinion.Ast.Expressions;

public class ScopeExpression : Expression
{
    public ScopeExpression(List<ScopeItem> members)
    {
        Members = members;
    }

    public List<ScopeItem> Members { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
