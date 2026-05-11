namespace TLPMinion.Ast.Expressions;

public sealed class StatementScopeItem : ScopeItem
{
    public StatementScopeItem(Expression statement)
    {
        Statement = statement;
    }

    public Expression Statement { get; }
}
