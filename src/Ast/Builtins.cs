using TLPMinion.Ast.Declarations;

namespace TLPMinion.Ast;

/// <summary>
/// Объект, предоставляющий доступ к встроенным символам языка.
/// </summary>
public static class Builtins
{
    public const string Print = "print";
    public const string Input = "input";
    public const string Length = "length";
    public const string Substring = "substring";
    public const string Int = "Int";
    public const string Float = "Float";
    public const string String = "String";
    public const string Void = "Void";

    /// <summary>Список встроенных функций.</summary>
    public static readonly IReadOnlyList<BuiltinFunction> Functions =
    [
        new(
            Length,
            [new BuiltinFunctionParameter("s", String)],
            Int),
        new(
            Substring,
            [
                new BuiltinFunctionParameter("s", String),
                new BuiltinFunctionParameter("start", Int),
                new BuiltinFunctionParameter("count", Int),
            ],
            String),
    ];
}
