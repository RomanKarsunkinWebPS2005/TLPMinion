using TLPMinion.Ast;
using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Ast.Statements;

namespace TLPMinion.Semantics.Passes;

public abstract class AbstractPass : IAstVisitor
{
    public virtual void Visit(ConstDeclaration declaration)
    {
        declaration.Initializer.Accept(this);
    }

    public virtual void Visit(VariableDeclaration declaration)
    {
        declaration.Initializer.Accept(this);
    }

    public virtual void Visit(AssignmentExpression expression)
    {
        expression.Left.Accept(this);
        expression.Right.Accept(this);
    }

    public virtual void Visit(BinaryExpression expression)
    {
        expression.Left.Accept(this);
        expression.Right.Accept(this);
    }

    public virtual void Visit(ConditionalExpression expression)
    {
        expression.Condition.Accept(this);
        expression.WhenTrue.Accept(this);
        expression.WhenFalse.Accept(this);
    }

    public virtual void Visit(FunctionCallExpression expression)
    {
        foreach (Expression argument in expression.Arguments)
        {
            argument.Accept(this);
        }
    }

    public virtual void Visit(IfStatement statement)
    {
        statement.Condition.Accept(this);
        statement.ThenBranch.Accept(this);
        statement.ElseBranch?.Accept(this);
    }

    public virtual void Visit(InputStatement statement)
    {
        statement.Target.Accept(this);
    }

    public virtual void Visit(IdentifierExpression expression)
    {
    }

    public virtual void Visit(LiteralExpression expression)
    {
    }

    public virtual void Visit(PrintStatement statement)
    {
        statement.Argument.Accept(this);
    }

    public virtual void Visit(ScopeExpression expression)
    {
        foreach (ScopeItem item in expression.Members)
        {
            switch (item)
            {
                case DeclarationScopeItem d:
                    d.Declaration.Accept(this);
                    break;
                case StatementScopeItem s:
                    s.Statement.Accept(this);
                    break;
                default:
                    throw new InvalidOperationException($"Неизвестный элемент области: {item.GetType().Name}.");
            }
        }
    }

    public virtual void Visit(UnaryExpression expression)
    {
        expression.Operand.Accept(this);
    }
}
