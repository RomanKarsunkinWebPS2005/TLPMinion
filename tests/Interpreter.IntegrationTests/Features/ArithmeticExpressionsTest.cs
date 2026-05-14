namespace Interpreter.IntegrationTests.Features;

public sealed class ArithmeticExpressionsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetPrintedArithmeticResults))]
    public void Can_evaluate_expressions(string code, string expected)
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    public static TheoryData<string, string> GetPrintedArithmeticResults()
    {
        return new TheoryData<string, string>
        {
            { "print(1 + 2);", "3" },
            { "print(10 - 3 - 2);", "5" },
            { "print(20 / 4 / 2);", "2" },
            { "print(2 + 3 * 4);", "14" },
            { "print((2 + 3) * 4);", "20" },
            { "print(10 % 3);", "1" },
            { "print(2.0 * 3.0);", "6" },
            { "print(5.0 - 1.5 - 0.5);", "3" },
            { "print(8.0 / 2.0 / 2.0);", "2" },
            { "print(2.0 ** 3.0 ** 2.0);", "512" },
            { "print(-(3.0 ** 2.0));", "-9" },
            { "print(+7);", "7" },
            { "print(-3 + 5);", "2" },
            { "print(1.0 / 0.0);", "Infinity" },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidSyntaxPrograms))]
    public void Reject_invalid_syntax_expressions(string code)
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Assert.Throws<UnexpectedLexemeException>(() =>
        {
            _ = interpreter.Execute(code);
        });
    }

    public static TheoryData<string> GetInvalidSyntaxPrograms()
    {
        return new TheoryData<string>
        {
            "(1 + 2;",
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidSemanticOrRuntimePrograms))]
    public void Reject_invalid_semantic_or_runtime_expressions(string code, Type exceptionType)
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Exception? ex = null;
        try
        {
            _ = interpreter.Execute(code);
        }
        catch (Exception e)
        {
            ex = e;
        }

        Assert.NotNull(ex);
        Assert.Equal(exceptionType, ex.GetType());
    }

    public static TheoryData<string, Type> GetInvalidSemanticOrRuntimePrograms()
    {
        return new TheoryData<string, Type>
        {
            { "1 + 1.0;", typeof(InvalidOperationException) },
            { "1.0 % 2.0;", typeof(InvalidOperationException) },
            { "2 ** 3;", typeof(InvalidOperationException) },
            { "1 / 0;", typeof(DivideByZeroException) },
        };
    }
}
