using TLPMinion.Ast.Declarations;

namespace TLPMinion.Ast;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public static class Builtins
{
    public const string Print = "print";
    public const string Input = "input";
    public const string Int = "Int";
    public const string Float = "Float";
    public const string String = "String";
    public const string Void = "Void";

    /// <summary>Список встроенных функций (пока пуст).</summary>
    public static readonly IReadOnlyList<BuiltinFunction> Functions = [];

    public static readonly IReadOnlyList<BuiltinType> Types =
    [
        new(Int),
        new(Float),
        new(String),
        new(Void),
    ];
}
