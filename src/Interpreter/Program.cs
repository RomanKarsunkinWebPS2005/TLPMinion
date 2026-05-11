namespace TLPMinion.Interpreter;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: TLPMinion.Interpreter <file-path>");
            return 1;
        }

        string sourcePath = args[0];
        if (!File.Exists(sourcePath))
        {
            Console.Error.WriteLine($"Error: source file '{sourcePath}' not found.");
            return 1;
        }

        try
        {
            string sourceCode = File.ReadAllText(sourcePath);

            ConsoleEnvironment environment = new();
            MinionInterpreter interpreter = new(environment);
            interpreter.Execute(sourceCode);

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Interpreter threw an {ex.GetType().Name}: {ex.Message}");
            return 1;
        }
    }
}
