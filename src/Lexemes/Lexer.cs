using System.Globalization;
using System.Text;

namespace TLPMinion.Lexemes;

public class Lexer
{
    private static readonly Dictionary<string, TokenType> _keywords = new()
    {
        { "const", TokenType.Const },
        { "let", TokenType.Let },
        { "var", TokenType.Var },
        { "print", TokenType.Print },
        { "input", TokenType.Input },
        { "Int", TokenType.TypeInt },
        { "Float", TokenType.TypeFloat },
    };

    private static readonly Dictionary<char, char> SimpleEscapes = new()
    {
        { 'n', '\n' },
        { '"', '\"' },
        { '\\', '\\' },
    };

    private readonly TextScanner _scanner;
    private bool _hasUnterminatedMultiLineComment;

    public Lexer(string code)
    {
        _scanner = new TextScanner(code);
    }

    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();
        if (_hasUnterminatedMultiLineComment)
        {
            _hasUnterminatedMultiLineComment = false;
            return new Token(TokenType.Error, "Незакрытый многострочный комментарий.");
        }

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char c = _scanner.Peek();
        if (char.IsAsciiLetter(c) || c == '_')
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(c))
        {
            return ParseNumericLiteral();
        }

        switch (c)
        {
            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);
            case ':':
                _scanner.Advance();
                return new Token(TokenType.Colon);
            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);
            case '(':
                _scanner.Advance();
                return new Token(TokenType.OpenParenthesis);
            case ')':
                _scanner.Advance();
                return new Token(TokenType.CloseParenthesis);
            case '{':
                _scanner.Advance();
                return new Token(TokenType.OpenBrace);
            case '}':
                _scanner.Advance();
                return new Token(TokenType.CloseBrace);
            case '=':
                _scanner.Advance();
                return new Token(TokenType.Assign);
            case '+':
                _scanner.Advance();
                return new Token(TokenType.Plus);
            case '-':
                _scanner.Advance();
                return new Token(TokenType.Minus);
            case '*':
                _scanner.Advance();
                if (!IsEndChar() && _scanner.Peek() == '*')
                {
                    _scanner.Advance();
                    return new Token(TokenType.Power);
                }

                return new Token(TokenType.Multiply);
            case '/':
                _scanner.Advance();
                return new Token(TokenType.Divide);
            case '%':
                _scanner.Advance();
                return new Token(TokenType.Percent);
        }

        _scanner.Advance();
        return new Token(TokenType.Error, $"Неизвестный токен: {c}");
    }

    private void SkipWhiteSpacesAndComments()
    {
        SkipWhiteSpaces();

        while (SkipSingleLineComment() || SkipMultiLineComment())
        {
            SkipWhiteSpaces();
            if (_hasUnterminatedMultiLineComment)
            {
                break;
            }
        }
    }

    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    private bool SkipSingleLineComment()
    {
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '/')
        {
            while (!_scanner.IsEnd() && _scanner.Peek() != '\n')
            {
                _scanner.Advance();
            }

            return true;
        }

        return false;
    }

    private bool SkipMultiLineComment()
    {
        if (_scanner.Peek() == '/' && _scanner.Peek(1) == '*')
        {
            _scanner.Advance(); // Пропускаем '/'.
            _scanner.Advance(); // Пропускаем '*'.

            while (!_scanner.IsEnd())
            {
                if (_scanner.Peek() == '*' && _scanner.Peek(1) == '/')
                {
                    _scanner.Advance(); // Пропускаем '*'.
                    _scanner.Advance(); // Пропускаем '/'.
                    return true;
                }

                _scanner.Advance();
            }

            _hasUnterminatedMultiLineComment = true;
            return true;
        }

        return false;
    }

    private Token ParseIdentifierOrKeyword()
    {
        string value = _scanner.Peek().ToString();
        _scanner.Advance();

        while (!_scanner.IsEnd())
        {
            char current = _scanner.Peek();
            if (!char.IsAsciiLetter(current) && !char.IsAsciiDigit(current) && current != '_')
            {
                break;
            }

            value += current;
            _scanner.Advance();
        }

        if (_keywords.TryGetValue(value, out TokenType keywordType))
        {
            return new Token(keywordType);
        }

        return new Token(TokenType.Identifier, value);
    }

    private Token ParseNumericLiteral()
    {
        string integerPart = ParseDigits();
        if (!_scanner.IsEnd() && _scanner.Peek() == '.' && char.IsAsciiDigit(_scanner.Peek(1)))
        {
            _scanner.Advance();
            string fractionalPart = ParseDigits();
            string literal = $"{integerPart}.{fractionalPart}";

            if (double.TryParse(literal, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                return new Token(TokenType.FloatLiteral, value);
            }

            return new Token(TokenType.Error, $"Некорректное вещественное число: {literal}");
        }

        return ParseIntLiteral(integerPart);
    }

    /// <summary>
    /// Разбирает литерал целого числа. Возвращает лексему Error, если число выходит за пределы типа данных int.
    /// </summary>
    private Token ParseIntLiteral(string digits)
    {
        if (int.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
        {
            return new Token(TokenType.IntLiteral, value);
        }

        return new Token(TokenType.Error, digits);
    }

    private string ParseDigits()
    {
        StringBuilder sb = new();
        while (!_scanner.IsEnd() && char.IsAsciiDigit(_scanner.Peek()))
        {
            sb.Append(_scanner.Peek());
            _scanner.Advance();
        }

        return sb.ToString();
    }

    private bool IsEndChar()
    {
        return _scanner.IsEnd();
    }
}