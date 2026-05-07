namespace TLPMinion.Lexemes;

/// <summary>
/// Сканирует исходный текст, предоставляя <see cref="Peek"/>, <see cref="Advance"/> и <see cref="IsEnd"/>.
/// </summary>
public class TextScanner
{
    private readonly string _input;
    private int _position;

    public TextScanner(string input)
    {
        _input = input;
    }

    /// <summary>
    /// Читает символ на N позиций вперёд от текущей (по умолчанию — текущий символ).
    /// </summary>
    public char Peek(int n = 0)
    {
        int position = _position + n;
        return position >= _input.Length ? '\0' : _input[position];
    }

    /// <summary>
    /// Сдвигает позицию на один символ вперёд.
    /// </summary>
    public void Advance()
    {
        _position++;
    }

    /// <summary>
    /// Достигнут ли конец входа.
    /// </summary>
    public bool IsEnd()
    {
        return _position >= _input.Length;
    }
}
