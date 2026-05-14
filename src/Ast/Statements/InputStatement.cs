using TLPMinion.Ast.Expressions;

namespace TLPMinion.Ast.Statements;

/// <summary>
/// Инструкция ввода: <c>input</c> <c>(</c> идентификатор <c>)</c> <c>;</c>.
/// </summary>
public sealed class InputStatement : Expression
{
    public InputStatement(IdentifierExpression target)
    {
        Target = target;
    }

    public IdentifierExpression Target { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
