using TLPMinion.Ast;
using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Ast.Statements;
using TLPMinion.Semantics.Helpers;

namespace TLPMinion.Semantics.Passes;

using VType = TLPMinion.Runtime.ValueType;

public sealed class TypeCheckingPass : AbstractPass
{
    public override void Visit(ScopeExpression expression)
    {
        VType lastStatementType = VType.Void;
        foreach (ScopeItem item in expression.Members)
        {
            switch (item)
            {
                case DeclarationScopeItem d:
                    d.Declaration.Accept(this);
                    break;
                case StatementScopeItem s:
                    s.Statement.Accept(this);
                    lastStatementType = s.Statement.ResultType;
                    break;
                default:
                    throw new InvalidOperationException($"Неизвестный элемент области: {item.GetType().Name}.");
            }
        }

        expression.ResultType = lastStatementType;
    }

    public override void Visit(ConstDeclaration declaration)
    {
        declaration.Initializer.Accept(this);
        VType expected = TypeHelpers.ParseTypeName(declaration.TypeName);
        TypeHelpers.AssertAssignable(declaration.Initializer.ResultType, expected, "const");
    }

    public override void Visit(VariableDeclaration declaration)
    {
        declaration.Initializer.Accept(this);
        VType expected = TypeHelpers.ParseTypeName(declaration.TypeName);
        TypeHelpers.AssertAssignable(declaration.Initializer.ResultType, expected, "переменной");
    }

    public override void Visit(AssignmentExpression expression)
    {
        expression.Left.Accept(this);
        expression.Right.Accept(this);
        if (expression.Left is not IdentifierExpression id)
        {
            throw new InvalidOperationException("Слева от '=' должно быть имя переменной.");
        }

        AbstractVariableDeclaration decl = id.Variable;
        if (decl is ConstDeclaration)
        {
            throw new InvalidOperationException($"Нельзя присваивать константе '{id.Name}'.");
        }

        if (decl is VariableDeclaration vd && !vd.IsMutable)
        {
            throw new InvalidOperationException($"Нельзя присваивать 'let'-переменной '{id.Name}'.");
        }

        TypeHelpers.AssertAssignable(expression.Right.ResultType, id.ResultType, "присваивание");
        expression.ResultType = expression.Right.ResultType;
    }

    public override void Visit(BinaryExpression expression)
    {
        expression.Left.Accept(this);
        expression.Right.Accept(this);
        VType l = expression.Left.ResultType;
        VType r = expression.Right.ResultType;
        expression.ResultType = expression.Operator switch
        {
            BinaryOperator.Add when l == VType.String && r == VType.String => VType.String,
            BinaryOperator.Add or BinaryOperator.Subtract or BinaryOperator.Multiply or BinaryOperator.Divide =>
                TypeHelpers.SameNumericArithmeticType(l, r),
            BinaryOperator.Modulo => TypeHelpers.ModuloType(l, r),
            BinaryOperator.Power => TypeHelpers.PowerType(l, r),
            _ => throw new InvalidOperationException($"Неизвестный оператор: {expression.Operator}"),
        };
    }

    public override void Visit(FunctionCallExpression expression)
    {
        foreach (Expression argument in expression.Arguments)
        {
            argument.Accept(this);
        }

        BuiltinFunction? builtin = FindBuiltin(expression.Name);
        if (builtin is null)
        {
            throw new InvalidOperationException(
                $"Вызов функции '{expression.Name}' не поддержан");
        }

        if (expression.Arguments.Count != builtin.Parameters.Count)
        {
            throw new InvalidOperationException(
                $"Функция '{expression.Name}' ожидает {builtin.Parameters.Count} аргумент(ов), передано {expression.Arguments.Count}.");
        }

        for (int i = 0; i < builtin.Parameters.Count; i++)
        {
            AbstractParameterDeclaration parameter = builtin.Parameters[i];
            VType expected = TypeHelpers.ParseTypeName(parameter.TypeName);
            TypeHelpers.AssertAssignable(expression.Arguments[i].ResultType, expected, parameter.Name);
        }

        expression.Function = builtin;
        expression.ResultType = TypeHelpers.ParseTypeName(builtin.ReturnTypeName);
    }

    public override void Visit(InputStatement statement)
    {
        statement.Target.Accept(this);
        IdentifierExpression id = statement.Target;
        AbstractVariableDeclaration decl = id.Variable;
        if (decl is ConstDeclaration)
        {
            throw new InvalidOperationException($"Нельзя использовать input для константы '{id.Name}'.");
        }

        if (decl is VariableDeclaration vd && !vd.IsMutable)
        {
            throw new InvalidOperationException($"Нельзя использовать input для 'let'-переменной '{id.Name}'.");
        }

        VType t = TypeHelpers.ParseTypeName(decl.TypeName);
        if (t != VType.Int && t != VType.Float && t != VType.String)
        {
            throw new InvalidOperationException("input допустим только для переменных типа Int, Float или String.");
        }

        statement.ResultType = VType.Void;
    }

    public override void Visit(PrintStatement statement)
    {
        statement.Argument.Accept(this);
        VType t = statement.Argument.ResultType;
        if (t != VType.Int && t != VType.Float && t != VType.String)
        {
            throw new InvalidOperationException("print ожидает аргумент типа Int, Float или String.");
        }

        statement.ResultType = VType.Void;
    }

    public override void Visit(IdentifierExpression expression)
    {
        expression.ResultType = TypeHelpers.ParseTypeName(expression.Variable.TypeName);
    }

    public override void Visit(LiteralExpression expression)
    {
        expression.ResultType = TypeHelpers.ParseTypeName(expression.TypeName);
    }

    public override void Visit(UnaryExpression expression)
    {
        expression.Operand.Accept(this);
        VType t = expression.Operand.ResultType;
        if (t != VType.Int && t != VType.Float)
        {
            throw new InvalidOperationException("Унарный + и - определены только для чисел.");
        }

        expression.ResultType = t;
    }

    private static BuiltinFunction? FindBuiltin(string name)
    {
        foreach (BuiltinFunction builtin in Builtins.Functions)
        {
            if (builtin.Name == name)
            {
                return builtin;
            }
        }

        return null;
    }
}
