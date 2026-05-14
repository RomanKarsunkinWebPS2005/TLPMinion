namespace Interpreter.IntegrationTests;

public sealed class EntryPointTest
{
    [Fact]
    public void Execute_returns_value_of_last_expression_from_smoke_sample_file()
    {
        FakeEnvironment environment = new();
        MinionInterpreter interpreter = new(environment);
        Value result = interpreter.Execute(Samples.GetSampleProgram("smoke.minion"));
        Assert.Equal(new Value(3), result);
        Assert.Equal(0, interpreter.ExitCode);
    }

    [Fact]
    public void Cli_returns_exit_code_1_on_semantic_error()
    {
        string path = Path.Combine(Path.GetTempPath(), $"minion_bad_{Guid.NewGuid():N}.minion");
        try
        {
            File.WriteAllText(path, "var x: Int = 1.0;");
            int exit = TLPMinion.Interpreter.Program.Main([path]);
            Assert.Equal(1, exit);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [Fact]
    public void Cli_returns_exit_code_0_on_valid_program()
    {
        string path = Path.Combine(Path.GetTempPath(), $"minion_ok_{Guid.NewGuid():N}.minion");
        try
        {
            File.WriteAllText(path, "1 + 1;");
            int exit = TLPMinion.Interpreter.Program.Main([path]);
            Assert.Equal(0, exit);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
