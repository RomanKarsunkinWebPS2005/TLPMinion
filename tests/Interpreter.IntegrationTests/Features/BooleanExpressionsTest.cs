namespace Interpreter.IntegrationTests.Features;

public sealed class BooleanExpressionsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetPrintedBooleanResults))]
    public void Can_evaluate_boolean_expressions(string code, string expected)
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        _ = interpreter.Execute(code);
        Assert.Equal(expected, environment.OutputBuffer);
        Assert.Equal(0, interpreter.ExitCode);
    }

    public static TheoryData<string, string> GetPrintedBooleanResults()
    {
        return new TheoryData<string, string>
        {
            { "print(true);", "true" },
            { "print(false);", "false" },
            { "print(!false);", "true" },
            { "print(1 == 1);", "true" },
            { "print(1 != 2);", "true" },
            { "print(1 < 2);", "true" },
            { "print(2 <= 2);", "true" },
            { "print(3 > 2);", "true" },
            { "print(2 >= 2);", "true" },
            { "print(1.0 == 1.0);", "true" },
            { "print(1.0 < 2.0);", "true" },
            { "print(true && false);", "false" },
            { "print(true || false);", "true" },
            { "print(true ? 1 : 2);", "1" },
            { "print(false ? 1 : 2);", "2" },
            { "print(false && ((1 / 0) == 1));", "false" },
            { "print(true || ((1 / 0) == 1));", "true" },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInvalidBooleanPrograms))]
    public void Reject_invalid_boolean_expressions(string code, Type exceptionType)
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

    public static TheoryData<string, Type> GetInvalidBooleanPrograms()
    {
        return new TheoryData<string, Type>
        {
            { "print(1 == 1.0);", typeof(InvalidOperationException) },
            { "print(true && ((1 / 0) == 1));", typeof(DivideByZeroException) },
        };
    }
}
