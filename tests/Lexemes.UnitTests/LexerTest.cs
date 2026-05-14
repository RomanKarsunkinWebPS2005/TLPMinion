using TLPMinion.Lexemes;

namespace Lexemes.UnitTests;

public class LexerTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetTokenizeIdentifiersAndKeywordsData))]
    [MemberData(nameof(GetTokenizeLiteralsData))]
    [MemberData(nameof(GetTokenizeOperatorsPunctuationData))]
    [MemberData(nameof(GetSkipWhitespacesAndCommentsData))]
    [MemberData(nameof(GetLexErrorsData))]
    public void Can_tokenize_lexemes(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < actual.Count; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

    public static TheoryData<string, List<Token>> GetTokenizeIdentifiersAndKeywordsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "alice Bob C00L D_2 _tmp x",
                [
                    new Token(TokenType.Identifier, "alice"),
                    new Token(TokenType.Identifier, "Bob"),
                    new Token(TokenType.Identifier, "C00L"),
                    new Token(TokenType.Identifier, "D_2"),
                    new Token(TokenType.Identifier, "_tmp"),
                    new Token(TokenType.Identifier, "x"),
                ]
            },
            {
                "count Count COUNT",
                [
                    new Token(TokenType.Identifier, "count"),
                    new Token(TokenType.Identifier, "Count"),
                    new Token(TokenType.Identifier, "COUNT"),
                ]
            },
            {
                "const let var print input Int Float String Void",
                [
                    new Token(TokenType.Const),
                    new Token(TokenType.Let),
                    new Token(TokenType.Var),
                    new Token(TokenType.Print),
                    new Token(TokenType.Input),
                    new Token(TokenType.TypeInt),
                    new Token(TokenType.TypeFloat),
                    new Token(TokenType.TypeString),
                    new Token(TokenType.TypeVoid),
                ]
            },
            {
                "printf inputValue Float32",
                [
                    new Token(TokenType.Identifier, "printf"),
                    new Token(TokenType.Identifier, "inputValue"),
                    new Token(TokenType.Identifier, "Float32"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeLiteralsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "0 42 56789 00173",
                [
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.IntLiteral, 42),
                    new Token(TokenType.IntLiteral, 56789),
                    new Token(TokenType.IntLiteral, 173),
                ]
            },
            {
                "3.14 0.0 10.25",
                [
                    new Token(TokenType.FloatLiteral, 3.14),
                    new Token(TokenType.FloatLiteral, 0.0),
                    new Token(TokenType.FloatLiteral, 10.25),
                ]
            },
            {
                "-7 +15",
                [
                    new Token(TokenType.Minus),
                    new Token(TokenType.IntLiteral, 7),
                    new Token(TokenType.Plus),
                    new Token(TokenType.IntLiteral, 15),
                ]
            },
            {
                "12.",
                [
                    new Token(TokenType.IntLiteral, 12),
                    new Token(TokenType.Error, "Неизвестный токен: ."),
                ]
            },
            {
                ".5",
                [
                    new Token(TokenType.Error, "Неизвестный токен: ."),
                    new Token(TokenType.IntLiteral, 5),
                ]
            },
            {
                "2147483648",
                [
                    new Token(TokenType.Error, "2147483648"),
                ]
            },
            {
                "\"\"",
                [
                    new Token(TokenType.StringLiteral, string.Empty),
                ]
            },
            {
                "\"hello\"",
                [
                    new Token(TokenType.StringLiteral, "hello"),
                ]
            },
            {
                "\"hello\\\"world\"",
                [
                    new Token(TokenType.StringLiteral, "hello\"world"),
                ]
            },
            {
                "\"a\\\\b\"",
                [
                    new Token(TokenType.StringLiteral, "a\\b"),
                ]
            },
            {
                "\"line\\n\"",
                [
                    new Token(TokenType.StringLiteral, "line\n"),
                ]
            },
            {
                "\"abc",
                [
                    new Token(TokenType.Error, "Незакрытая строковая константа."),
                ]
            },
            {
                "\"x\\y\"",
                [
                    new Token(TokenType.Error, "Неизвестная escape-последовательность: \\y"),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Error, "Незакрытая строковая константа."),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetTokenizeOperatorsPunctuationData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x + y / (10 - z * 2) % 3",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Plus),
                    new Token(TokenType.Identifier, "y"),
                    new Token(TokenType.Divide),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.IntLiteral, 10),
                    new Token(TokenType.Minus),
                    new Token(TokenType.Identifier, "z"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.IntLiteral, 2),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Percent),
                    new Token(TokenType.IntLiteral, 3),
                ]
            },
            {
                "2 ** 8",
                [
                    new Token(TokenType.IntLiteral, 2),
                    new Token(TokenType.Power),
                    new Token(TokenType.IntLiteral, 8),
                ]
            },
            {
                "a * b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Multiply),
                    new Token(TokenType.Identifier, "b"),
                ]
            },
            {
                "(foo(x);0),{x:Int}=1",
                [
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "foo"),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Semicolon),
                    new Token(TokenType.IntLiteral, 0),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Comma),
                    new Token(TokenType.OpenBrace),
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Colon),
                    new Token(TokenType.TypeInt),
                    new Token(TokenType.CloseBrace),
                    new Token(TokenType.Assign),
                    new Token(TokenType.IntLiteral, 1),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetSkipWhitespacesAndCommentsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "x \t\r\n\fy",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "y"),
                ]
            },
            {
                "x // comment\n y",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Identifier, "y"),
                ]
            },
            {
                "a /* comment */ b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Identifier, "b"),
                ]
            },
            {
                "a /* outer /* inner */ b",
                [
                    new Token(TokenType.Identifier, "a"),
                    new Token(TokenType.Identifier, "b"),
                ]
            },
        };
    }

    public static TheoryData<string, List<Token>> GetLexErrorsData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                "@x",
                [
                    new Token(TokenType.Error, "Неизвестный токен: @"),
                    new Token(TokenType.Identifier, "x"),
                ]
            },
            {
                "x /* unclosed",
                [
                    new Token(TokenType.Identifier, "x"),
                    new Token(TokenType.Error, "Незакрытый многострочный комментарий."),
                ]
            },
        };
    }

    private static List<Token> Tokenize(string code)
    {
        List<Token> result = [];
        Lexer lexer = new(code);
        for (Token token = lexer.ParseToken(); token.Type != TokenType.EndOfFile; token = lexer.ParseToken())
        {
            result.Add(token);
        }

        return result;
    }
}
