using TLPMinion.Ast.Attributes;

using ValueType = TLPMinion.Runtime.ValueType;

namespace TLPMinion.Ast.Expressions;

/// <summary>
/// Абстрактный подкласс выражения.
/// </summary>
public abstract class Expression : AstNode
{
    private AstAttribute<ValueType> _resultType;

    /// <summary>
    /// Тип результата выражения (задаётся проходом проверки типов).
    /// </summary>
    public ValueType ResultType
    {
        get => _resultType.Get();

        set => _resultType.Set(value);
    }
}
