using System.Text;

using TLPMinion.VirtualMachine;

namespace TLPMinion.Interpreter;

public class ConsoleEnvironment : IEnvironment
{
    public string ReadWord()
    {
        int c;
        while ((c = Console.Read()) >= 0 && char.IsWhiteSpace((char)c))
        {
        }

        if (c < 0)
        {
            throw new EndOfStreamException("Нет данных во входном потоке.");
        }

        StringBuilder sb = new();
        sb.Append((char)c);
        while ((c = Console.Read()) >= 0 && !char.IsWhiteSpace((char)c))
        {
            sb.Append((char)c);
        }

        return sb.ToString();
    }

    public void Print(string text)
    {
        Console.Write(text);
    }
}
