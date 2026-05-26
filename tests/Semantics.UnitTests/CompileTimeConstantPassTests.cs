using TLPMinion.Ast;
using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Runtime;
using TLPMinion.Semantics.Passes;

namespace Semantics.UnitTests;

public sealed class CompileTimeConstantPassTests
{
    [Fact]
    public void Const_chain_of_int_refs_and_arithmetic_folded()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("""
            const A: Int = 10;
            const B: Int = A + 5;
            const C: Int = B * 2 - A;
            """);
        SemanticsTestHelpers.RunSemantics(program);

        Assert.Equal(new Value(20), SemanticsTestHelpers.RequireConst(program, "C").CompileTimeValue);
    }

    [Fact]
    public void Const_int_unary_plus_and_minus_folded()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("""
            const A: Int = +7;
            const B: Int = -A;
            const C: Int = -(-3);
            """);
        SemanticsTestHelpers.RunSemantics(program);

        Assert.Equal(new Value(7), SemanticsTestHelpers.RequireConst(program, "A").CompileTimeValue);
        Assert.Equal(new Value(-7), SemanticsTestHelpers.RequireConst(program, "B").CompileTimeValue);
        Assert.Equal(new Value(3), SemanticsTestHelpers.RequireConst(program, "C").CompileTimeValue);
    }

    [Fact]
    public void Const_int_divide_and_modulo_folded()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("""
            const Q: Int = 7 / 2;
            const R: Int = 7 % 2;
            """);
        SemanticsTestHelpers.RunSemantics(program);

        Assert.Equal(new Value(3), SemanticsTestHelpers.RequireConst(program, "Q").CompileTimeValue);
        Assert.Equal(new Value(1), SemanticsTestHelpers.RequireConst(program, "R").CompileTimeValue);
    }

    [Fact]
    public void Const_float_power_literal_folded()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("const P: Float = 2.0 ** 3.0;");
        SemanticsTestHelpers.RunSemantics(program);

        Assert.Equal(new Value(8.0), SemanticsTestHelpers.RequireConst(program, "P").CompileTimeValue);
    }

    [Fact]
    public void Const_float_unary_minus_folded()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("const X: Float = -2.5;");
        SemanticsTestHelpers.RunSemantics(program);

        Assert.Equal(new Value(-2.5), SemanticsTestHelpers.RequireConst(program, "X").CompileTimeValue);
    }

    [Fact]
    public void Const_int_divide_by_zero_throws()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("const X: Int = 1 / 0;");
        Assert.Throws<DivideByZeroException>(() => SemanticsTestHelpers.RunSemantics(program));
    }

    [Fact]
    public void Const_int_modulo_by_zero_throws()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("const X: Int = 1 % 0;");
        Assert.Throws<DivideByZeroException>(() => SemanticsTestHelpers.RunSemantics(program));
    }

    [Fact]
    public void Const_float_divide_by_zero_throws()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("const X: Float = 1.0 / 0.0;");
        Assert.Throws<DivideByZeroException>(() => SemanticsTestHelpers.RunSemantics(program));
    }

    [Fact]
    public void Const_ref_forward_declaration_fails_resolve_names()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("""
            const B: Int = A;
            const A: Int = 1;
            """);
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => SemanticsTestHelpers.RunSemantics(program));
        Assert.Contains("A", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Const_ref_non_const_variable_throws_only_const_refs_allowed()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("""
            const A: Int = 1;
            var v: Int = 2;
            const B: Int = v;
            """);
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => SemanticsTestHelpers.RunSemantics(program));
        Assert.Contains("v", ex.Message, StringComparison.Ordinal);
        Assert.Contains("констант", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Const_int_overflow_in_add_throws()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("""
            const A: Int = 2000000000;
            const B: Int = 2000000000;
            const C: Int = A + B;
            """);
        Assert.Throws<OverflowException>(() => SemanticsTestHelpers.RunSemantics(program));
    }

    [Fact]
    public void Const_unary_minus_on_string_throws()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("""const S: String = -"x";""");
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => SemanticsTestHelpers.RunSemantics(program));
        Assert.Contains("чисел", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Const_type_mismatch_int_initializer_for_float_const_throws()
    {
        ScopeExpression program = SemanticsTestHelpers.ParseProgram("const X: Float = 1;");
        Assert.Throws<InvalidOperationException>(() => SemanticsTestHelpers.RunSemantics(program));
    }

    [Fact]
    public void Ref_to_const_not_yet_folded_throws_declared_above_message()
    {
        ConstDeclaration declA = new("A", Builtins.Int, new LiteralExpression(Builtins.Int, "1"));
        IdentifierExpression refA = new("A") { Variable = declA };
        ConstDeclaration declB = new("B", Builtins.Int, refA);

        ScopeExpression program = new(
        [
            new DeclarationScopeItem(declB),
            new DeclarationScopeItem(declA),
        ]);

        CompileTimeConstantPass pass = new();
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => program.Accept(pass));

        Assert.Contains("A", ex.Message, StringComparison.Ordinal);
        Assert.Contains("выше", ex.Message, StringComparison.Ordinal);
    }
}
