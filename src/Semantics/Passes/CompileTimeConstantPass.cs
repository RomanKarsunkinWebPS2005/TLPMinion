using System.Globalization;

using TLPMinion.Ast;
using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Runtime;
using TLPMinion.Semantics.Helpers;

namespace TLPMinion.Semantics.Passes;

using VType = TLPMinion.Runtime.ValueType;

public sealed class CompileTimeConstantPass : AbstractPass
{
    public override void Visit(ConstDeclaration declaration)
    {
        Value computed = Evaluate(declaration.Initializer);
        VType got = ValueToSemanticType(computed);
        TypeHelpers.AssertAssignable(got, TypeHelpers.ToValueType(declaration.TypeName), "const");
        declaration.CompileTimeValue = computed;
    }

    private static Value Evaluate(Expression expression)
    {
        return expression switch
        {
            LiteralExpression lit => lit.TypeName switch
            {
                Builtins.Int => new Value(int.Parse(lit.Lexeme, CultureInfo.InvariantCulture)),
                Builtins.Float => new Value(double.Parse(lit.Lexeme, CultureInfo.InvariantCulture)),
                Builtins.String => new Value(lit.Lexeme),
                _ => throw new InvalidOperationException("В const допускаются только литералы Int, Float и String."),
            },
            IdentifierExpression id => EvalConstRef(id),
            UnaryExpression u => EvalUnary(u),
            BinaryExpression b => ApplyBinary(b.Operator, Evaluate(b.Left), Evaluate(b.Right)),
            _ => throw new InvalidOperationException(
                "Инициализатор const должен вычисляться на этапе компиляции: " +
                "литералы, ссылки на ранее объявленные const, строковая конкатенация + и числовые операторы (+, -, *, /, %, **)."),
        };
    }

    private static VType ValueToSemanticType(Value v)
    {
        if (v.IsInt())
        {
            return VType.Int;
        }

        if (v.IsDouble())
        {
            return VType.Float;
        }

        if (v.IsString())
        {
            return VType.String;
        }

        throw new InvalidOperationException("Неподдерживаемое значение в const.");
    }

    private static Value EvalConstRef(IdentifierExpression id)
    {
        if (id.Variable is not ConstDeclaration cd)
        {
            throw new InvalidOperationException(
                $"В инициализаторе const допускаются только ссылки на другие константы (не '{id.Name}').");
        }

        if (cd.CompileTimeValue is not { } v)
        {
            throw new InvalidOperationException(
                $"Константа '{id.Name}' должна быть объявлена выше по тексту, чтобы использовать её здесь.");
        }

        return v;
    }

    private static Value EvalUnary(UnaryExpression u)
    {
        Value inner = Evaluate(u.Operand);
        if (inner.IsString())
        {
            throw new InvalidOperationException("Унарные операторы недопустимы для строковых значений в const.");
        }

        return u.Operator switch
        {
            UnaryOperator.Plus => inner,
            UnaryOperator.Minus => inner.IsInt()
                ? new Value(checked(-inner.AsInt()))
                : new Value(-inner.AsDouble()),
            _ => throw new InvalidOperationException($"Оператор {u.Operator} недопустим в константном выражении."),
        };
    }

    private static Value ApplyBinary(BinaryOperator op, Value l, Value r)
    {
        if (op == BinaryOperator.Add && l.IsString() && r.IsString())
        {
            return new Value(l.AsString() + r.AsString());
        }

        if (l.IsString() || r.IsString())
        {
            throw new InvalidOperationException("Для строк в const допускается только конкатенация через +.");
        }

        bool bothInt = l.IsInt() && r.IsInt();
        return op switch
        {
            BinaryOperator.Add => bothInt
                ? new Value(checked(l.AsInt() + r.AsInt()))
                : new Value(l.AsDouble() + r.AsDouble()),
            BinaryOperator.Subtract => bothInt
                ? new Value(checked(l.AsInt() - r.AsInt()))
                : new Value(l.AsDouble() - r.AsDouble()),
            BinaryOperator.Multiply => bothInt
                ? new Value(checked(l.AsInt() * r.AsInt()))
                : new Value(l.AsDouble() * r.AsDouble()),
            BinaryOperator.Divide => bothInt
                ? new Value(DivInt(l.AsInt(), r.AsInt()))
                : new Value(DivFloat(l.AsDouble(), r.AsDouble())),
            BinaryOperator.Modulo => new Value(ModInt(l.AsInt(), r.AsInt())),
            BinaryOperator.Power => new Value(Math.Pow(l.AsDouble(), r.AsDouble())),
            _ => throw new InvalidOperationException($"Оператор {op} недопустим в константном выражении."),
        };
    }

    private static int DivInt(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Деление на ноль в константном выражении.");
        }

        return a / b;
    }

    private static double DivFloat(double a, double b)
    {
        if (Math.Abs(b) < double.Epsilon)
        {
            throw new DivideByZeroException("Деление на ноль в константном выражении.");
        }

        return a / b;
    }

    private static int ModInt(int a, int b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException("Остаток от деления на ноль в константном выражении.");
        }

        return a % b;
    }
}
