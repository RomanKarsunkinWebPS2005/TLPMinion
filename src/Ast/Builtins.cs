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

    public static readonly IReadOnlyList<BuiltinFunction> Functions =
    [
        new(
            Print,
            [new BuiltinFunctionParameter("value", Int)],
            Int
        ),
        new(
            Print,
            [new BuiltinFunctionParameter("value", Float)],
            Float
        ),
        new(
            Input,
            [new BuiltinFunctionParameter("type", Int)],
            Int
        ),
        new(
            Input,
            [new BuiltinFunctionParameter("type", Float)],
            Float
        ),
    ];

    public static readonly IReadOnlyList<BuiltinType> Types =
    [
        new(Int),
        new(Float),
    ];
}
