namespace Interpreter.IntegrationTests;

public sealed class StringsTest
{
    [Fact]
    public void Print_string_literal()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""print("hello");""");
        Assert.Equal("hello", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Print_string_concatenation()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""print("a" + "b" + "c");""");
        Assert.Equal("abc", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Print_string_with_escape_newline()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""print("x\ny");""");
        Assert.Equal("x\ny", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Const_string_literal_concat_and_print()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            const A: String = "foo";
            const B: String = A + "-" + "bar";
            print(B);
            """);
        Assert.Equal("foo-bar", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Let_string_and_concat_in_print()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            let s: String = "ok";
            print(s + "!");
            """);
        Assert.Equal("ok!", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Var_string_assign_and_print()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            var t: String;
            t = "z";
            print(t + t);
            """);
        Assert.Equal("zz", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void String_plus_int_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""print("a" + 1);""");
        });
    }

    [Fact]
    public void String_times_string_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""print("a" * "b");""");
        });
    }

    [Fact]
    public void Unary_minus_on_string_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""print(-"x");""");
        });
    }

    [Fact]
    public void Init_string_var_with_int_literal_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("var s: String = 1;");
        });
    }

    [Fact]
    public void Assign_int_to_string_var_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                var s: String = "a";
                s = 2;
                """);
        });
    }

    [Fact]
    public void Const_string_initializer_with_invalid_operator_throws()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""const c: String = "a" * "b";""");
        });
    }

    [Fact]
    public void Print_length_of_string_literal()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""print(length("abc"));""");
        Assert.Equal("3", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Print_substring_of_string_literal()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""print(substring("hello", 1, 3));""");
        Assert.Equal("ell", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Length_nested_in_substring_and_concat()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute("""
            var s: String = "ab";
            print(substring(s + "cd", 1, length(s)));
            """);
        Assert.Equal("bc", environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Length_with_int_argument_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""print(length(1));""");
        });
    }

    [Fact]
    public void Substring_with_string_start_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""print(substring("x", "1", 1));""");
        });
    }

    [Fact]
    public void Length_in_const_initializer_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""const n: Int = length("a");""");
        });
    }
}
