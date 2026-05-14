using TLPMinion.Ast.Declarations;

namespace TLPMinion.Ast.Expressions;

public sealed class DeclarationScopeItem : ScopeItem
{
    public DeclarationScopeItem(Declaration declaration)
    {
        Declaration = declaration;
    }

    public Declaration Declaration { get; }
}
