namespace TLPMinion.Ast.Declarations;

public sealed class BuiltinFunction : AbstractFunctionDeclaration
{
    public BuiltinFunction(string name, IReadOnlyList<BuiltinFunctionParameter> parameters, string returnTypeName)
        : base(name, parameters)
    {
        ReturnTypeName = returnTypeName;
    }

    public string ReturnTypeName { get; }

    public override void Accept(IAstVisitor visitor)
    {
        throw new InvalidOperationException($"Visitor cannot be applied to {GetType()}");
    }
}
