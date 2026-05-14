namespace Interpreter.IntegrationTests;

public sealed class VariablesTest
{
    [Fact]
    public void Var_float_without_explicit_initializer_then_assign_and_print()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            var f: Float;
            f = 3.14;
            print(f);
            """);
        Assert.Equal("3.14", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Const_int_literal_and_print()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            const N: Int = 42;
            print(N);
            """);
        Assert.Equal("42", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Assignment_to_let_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                let x: Int = 1;
                x = 2;
                """);
        });
    }

    [Fact]
    public void Assignment_to_const_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                const c: Int = 1;
                c = 2;
                """);
        });
    }

    [Fact]
    public void Inner_block_shadows_outer_name()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            let a: Int = 1;
            {
              let a: Int = 2;
              print(a);
            }
            print(a);
            """);
        Assert.Equal("21", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Assign_outer_var_from_inner_block_without_redeclaration()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            var x: Int = 1;
            {
              x = 5;
            }
            print(x);
            """);
        Assert.Equal("5", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Duplicate_declaration_in_same_scope_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                var x: Int = 1;
                var x: Int = 2;
                """);
        });
    }

    [Fact]
    public void Assignment_to_unknown_identifier_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("print(z);");
        });
    }

    [Fact]
    public void Init_with_float_literal_to_int_var_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("var x: Int = 1.0;");
        });
    }

    [Fact]
    public void Assign_float_to_int_var_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                var x: Int = 1;
                x = 2.0;
                """);
        });
    }
}
