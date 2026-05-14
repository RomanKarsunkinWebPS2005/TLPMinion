using TLPMinion.Ast.Expressions;
using TLPMinion.Runtime;

namespace TLPMinion.Ast.Declarations;

public class ConstDeclaration : AbstractVariableDeclaration
{
    public ConstDeclaration(string name, string typeName, Expression initializer)
        : base(name)
    {
        TypeName = typeName;
        Initializer = initializer;
    }

    public override string TypeName { get; }

    public Expression Initializer { get; }

    public Value? CompileTimeValue { get; set; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
