using TLPMinion.Ast.Expressions;

namespace TLPMinion.Ast.Declarations;

public class VariableDeclaration : AbstractVariableDeclaration
{
    public VariableDeclaration(string name, string typeName, Expression initializer, bool isMutable)
        : base(name)
    {
        TypeName = typeName;
        Initializer = initializer;
        IsMutable = isMutable;
    }

    public override string TypeName { get; }

    public Expression Initializer { get; }

    public bool IsMutable { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
