using TLPMinion.Ast.Attributes;
using TLPMinion.Ast.Declarations;

namespace TLPMinion.Ast.Expressions;

public sealed class IdentifierExpression : Expression
{
    private AstAttribute<AbstractVariableDeclaration> _variable;

    public IdentifierExpression(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public AbstractVariableDeclaration Variable
    {
        get => _variable.Get();
        set => _variable.Set(value);
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
