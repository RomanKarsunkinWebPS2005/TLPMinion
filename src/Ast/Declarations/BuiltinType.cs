namespace TLPMinion.Ast.Declarations;

public class BuiltinType : AbstractTypeDeclaration
{
    public BuiltinType(string name)
        : base(name)
    {
    }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}
