using TLPMinion.Ast.Expressions;

namespace TLPMinion.Ast.Statements;

/// <summary>
/// Условная инструкция: <c>if</c> <c>(</c> выражение <c>)</c> блок [ <c>else</c> ( блок | if-statement ) ].
/// </summary>
public sealed class IfStatement : Expression
{
    public IfStatement(Expression condition, ScopeExpression thenBranch, Expression? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public ScopeExpression ThenBranch { get; }

    public Expression? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
