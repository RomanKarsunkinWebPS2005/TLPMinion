using TLPMinion.Lexemes;

namespace TLPMinion.Parser;

#pragma warning disable RCS1194
public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(Token actual, TokenType expected)
        : base($"Неожиданная лексема {actual}, ожидалась {expected}")
    {
        Actual = actual.Type;
        Expected = [expected];
    }

    public UnexpectedLexemeException(Token actual, IReadOnlyList<TokenType> expected)
        : base($"Неожиданная лексема {actual}, ожидалась одна из: {string.Join(", ", expected)}")
    {
        Actual = actual.Type;
        Expected = expected;
    }

    public TokenType Actual { get; }

    public IReadOnlyList<TokenType> Expected { get; }
}
#pragma warning restore RCS1194
