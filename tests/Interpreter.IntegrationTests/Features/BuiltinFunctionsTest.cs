namespace Interpreter.IntegrationTests;

public sealed class BuiltinFunctionsTest
{
    [Fact]
    public void Input_assigns_int_from_word()
    {
        FakeEnvironment environment = new();
        environment.AddInput("99");
        MinionInterpreter interpreter = new(environment);
        Value result = interpreter.Execute("""
            var a: Int;
            input(a);
            a;
            """);
        Assert.Equal(new Value(99), result);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Input_assigns_float_from_word()
    {
        FakeEnvironment environment = new();
        environment.AddInput("3.5");
        MinionInterpreter interpreter = new(environment);
        Value result = interpreter.Execute("""
            var x: Float;
            input(x);
            x;
            """);
        Assert.Equal(new Value(3.5), result);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Input_assigns_string_from_word()
    {
        FakeEnvironment environment = new();
        environment.AddInput("hello");
        MinionInterpreter interpreter = new(environment);
        Value result = interpreter.Execute("""
            var s: String;
            input(s);
            s;
            """);
        Assert.Equal(new Value("hello"), result);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Input_string_when_no_words_throws_end_of_stream()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<EndOfStreamException>(() =>
        {
            _ = interpreter.Execute("""
                var s: String;
                input(s);
                """);
        });
    }

    [Fact]
    public void Input_invalid_int_throws_format_exception()
    {
        FakeEnvironment environment = new();
        environment.AddInput("not_int");
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<FormatException>(() =>
        {
            _ = interpreter.Execute("""
                var a: Int;
                input(a);
                """);
        });
    }

    [Fact]
    public void Input_when_no_words_throws_end_of_stream()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<EndOfStreamException>(() =>
        {
            _ = interpreter.Execute("""
                var a: Int;
                input(a);
                """);
        });
    }

    [Fact]
    public void Input_into_let_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        environment.AddInput("1");
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                let x: Int = 0;
                input(x);
                """);
        });
    }

    [Fact]
    public void Input_into_const_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        environment.AddInput("1");
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                const c: Int = 0;
                input(c);
                """);
        });
    }

    [Fact]
    public void Input_into_let_string_throws_semantic_error()
    {
        FakeEnvironment environment = new();
        environment.AddInput("x");
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = interpreter.Execute("""
                let s: String = "";
                input(s);
                """);
        });
    }
}
