using TLPMinion.Ast.Expressions;

namespace TLPMinion.Ast.Declarations;

public class VariableDeclaration : AbstractVariableDeclaration
{
    public VariableDeclaration(string name, string typeName, Expression initializer)
        : base(name)
    {
        TypeName = typeName;
        Initializer = initializer;
    }

    public string TypeName { get; }

    public Expression Initializer { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
