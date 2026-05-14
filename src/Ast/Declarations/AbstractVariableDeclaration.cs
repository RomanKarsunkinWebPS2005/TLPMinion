namespace TLPMinion.Ast.Declarations;

public abstract class AbstractVariableDeclaration : Declaration
{
    protected AbstractVariableDeclaration(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract string TypeName { get; }
}
