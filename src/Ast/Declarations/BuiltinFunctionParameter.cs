namespace TLPMinion.Ast.Declarations;

public class BuiltinFunctionParameter : AbstractParameterDeclaration
{
    public BuiltinFunctionParameter(string name, string typeName)
        : base(name)
    {
        TypeName = typeName;
    }

    public override string TypeName { get; }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}
