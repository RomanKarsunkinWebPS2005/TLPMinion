using System.Runtime.CompilerServices;

namespace TLPMinion.Ast.Attributes;

/// <summary>
/// Атрибут AST задаётся один раз на фазе семантики.
/// </summary>
public struct AstAttribute<T>
{
    private T _value;
    private bool _initialized;

    public T Get([CallerMemberName] string? memberName = null)
    {
        if (!_initialized)
        {
            throw new InvalidOperationException($"Атрибут {memberName} типа {typeof(T)} не задан.");
        }

        return _value;
    }

    public void Set(T value, [CallerMemberName] string? memberName = null)
    {
        if (_initialized)
        {
            throw new InvalidOperationException($"Атрибут {memberName} типа {typeof(T)} уже задан.");
        }

        _value = value;
        _initialized = true;
    }
}
