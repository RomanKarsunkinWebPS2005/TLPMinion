using TLPMinion.Ast;

namespace TLPMinion.Semantics.Helpers;

using VType = TLPMinion.Runtime.ValueType;

internal static class TypeHelpers
{
    public static VType ToValueType(string name)
    {
        return name switch
        {
            Builtins.Int => VType.Int,
            Builtins.Float => VType.Float,
            Builtins.String => VType.String,
            Builtins.Bool => VType.Bool,
            Builtins.Void => VType.Void,
            _ => throw new InvalidOperationException($"Неизвестное имя типа '{name}'."),
        };
    }

    public static void AssertAssignable(VType actual, VType expected, string context)
    {
        if (actual != expected)
        {
            throw new InvalidOperationException(
                $"Несовпадение типов ({context}): ожидалось {expected}, получено {actual}.");
        }
    }

    public static VType SameNumericArithmeticType(VType l, VType r)
    {
        if (l == VType.Int && r == VType.Int)
        {
            return VType.Int;
        }

        if (l == VType.Float && r == VType.Float)
        {
            return VType.Float;
        }

        throw new InvalidOperationException(
            "Операторы +, -, *, / требуют операнды одного типа (оба Int или оба Float); " +
            "неявные преобразования не поддерживаются.");
    }

    public static VType ModuloType(VType l, VType r)
    {
        if (l != VType.Int || r != VType.Int)
        {
            throw new InvalidOperationException("Оператор '%' определён только для Int.");
        }

        return VType.Int;
    }

    public static VType PowerType(VType l, VType r)
    {
        if (l != VType.Float || r != VType.Float)
        {
            throw new InvalidOperationException("Оператор '**' определён только для Float.");
        }

        return VType.Float;
    }

    public static VType SameEqualityOperandType(VType l, VType r)
    {
        if (l == r && l is VType.Int or VType.Float or VType.String or VType.Bool)
        {
            return VType.Bool;
        }

        throw new InvalidOperationException(
            "Операторы == и != требуют операнды одного типа (Int, Float, String или Bool).");
    }

    public static VType SameOrderingOperandType(VType l, VType r)
    {
        if (l == VType.Int && r == VType.Int)
        {
            return VType.Bool;
        }

        if (l == VType.Float && r == VType.Float)
        {
            return VType.Bool;
        }

        if (l == VType.String && r == VType.String)
        {
            return VType.Bool;
        }

        throw new InvalidOperationException(
            "Операторы <, <=, >, >= требуют операнды одного типа (оба Int, оба Float или обе String).");
    }

    public static VType LogicalOperandType(VType l, VType r)
    {
        if (l == VType.Bool && r == VType.Bool)
        {
            return VType.Bool;
        }

        throw new InvalidOperationException("Операторы && и || требуют операнды типа Bool.");
    }
}
