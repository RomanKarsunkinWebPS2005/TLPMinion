using System.Globalization;

namespace TLPMinion.Runtime;

public sealed class Value : IEquatable<Value>
{
    public static readonly Value Void = new(VoidValue.Value);

    private readonly object _value;

    public Value(int value)
    {
        _value = value;
    }

    public Value(double value)
    {
        _value = value;
    }

    public Value(string value)
    {
        _value = value;
    }

    private Value(object value)
    {
        _value = value;
    }

    public bool IsVoid() => _value is VoidValue;

    public bool IsInt() => _value is int;

    public bool IsDouble() => _value is double;

    public int AsInt()
    {
        return _value switch
        {
            int i => i,
            double d => checked((int)d),
            _ => throw new InvalidOperationException($"Значение не целое: {_value}"),
        };
    }

    public double AsDouble()
    {
        return _value switch
        {
            int i => i,
            double d => d,
            _ => throw new InvalidOperationException($"Значение не число: {_value}"),
        };
    }

    public string AsString()
    {
        return _value switch
        {
            string s => s,
            _ => throw new InvalidOperationException($"Значение не строка операнда: {_value}"),
        };
    }

    public string ToDisplayString()
    {
        return _value switch
        {
            int i => i.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            VoidValue => "<void>",
            string s => s,
            _ => _value.ToString() ?? string.Empty,
        };
    }

    public bool Equals(Value? other)
    {
        if (other is null)
        {
            return false;
        }

        return _value switch
        {
            int i => other._value is int oi && i == oi,
            double d => other._value is double od && Math.Abs(d - od) < double.Epsilon,
            VoidValue => other._value is VoidValue,
            string s => other._value is string os && s == os,
            _ => ReferenceEquals(_value, other._value),
        };
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Value);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public override string ToString()
    {
        return ToDisplayString();
    }
}
