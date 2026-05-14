using System.Text;

using TLPMinion.VirtualMachine;

namespace TLPMinion.Tests.TestLibrary.TestDoubles;

public sealed class FakeEnvironment : IEnvironment
{
    private readonly Queue<string> _words = new();
    private readonly StringBuilder _output = new();

    public string Output => _output.ToString();

    public void AddInput(string text)
    {
        foreach (string word in text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
        {
            _words.Enqueue(word);
        }
    }

    public string ReadWord()
    {
        if (!_words.TryDequeue(out string? word))
        {
            throw new EndOfStreamException("Нет данных во входном потоке (FakeEnvironment).");
        }

        return word;
    }

    public void Print(string text)
    {
        _output.Append(text);
    }
}
