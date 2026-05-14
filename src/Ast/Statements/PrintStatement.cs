using TLPMinion.Ast.Expressions;

namespace TLPMinion.Ast.Statements;

/// <summary>
/// Инструкция вывода: <c>print</c> <c>(</c> выражение <c>)</c> <c>;</c>.
/// </summary>
public sealed class PrintStatement : Expression
{
    public PrintStatement(Expression argument)
    {
        Argument = argument;
    }

    public Expression Argument { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
