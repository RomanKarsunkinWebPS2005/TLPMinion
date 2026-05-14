namespace Interpreter.IntegrationTests;

public class ProgramsTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetPrograms))]
    public void Can_exec_program(string prg, string expected, int exitCode)
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);

        _ = interpreter.Execute(Samples.GetSampleProgram(prg));
        Assert.Equal(expected, environment.OutputBuffer);
        Assert.Equal(exitCode, interpreter.ExitCode);
    }

    public static TheoryData<string, string, int> GetPrograms()
    {
        return new TheoryData<string, string, int>
        {
            { "smoke.minion", "", 0 },
            { "print_literal.minion", "423.143549", 0 },
            { "string_concat.minion", "abcd", 0 },
        };
    }
}
