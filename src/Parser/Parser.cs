using TLPMinion.Ast;
using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Ast.Statements;
using TLPMinion.Lexemes;

namespace TLPMinion.Parser;

public class Parser
{
    private readonly TokenStream _tokens;

    public Parser(string source)
    {
        _tokens = new TokenStream(source);
    }

    public Expression ParseProgram()
    {
        List<ScopeItem> members = [];

        while (!Is(TokenType.EndOfFile))
        {
            if (IsDeclarationStart(_tokens.Peek().Type))
            {
                members.Add(new DeclarationScopeItem(ParseDeclaration()));
                continue;
            }

            members.Add(new StatementScopeItem(ParseStatementExpression()));
        }

        return new ScopeExpression(members);
    }

    private Declaration ParseDeclaration()
    {
        return _tokens.Peek().Type switch
        {
            TokenType.Const => ParseConstDeclaration(),
            TokenType.Let => ParseVariableDeclaration(),
            TokenType.Var => ParseVariableDeclaration(),
            _ => throw new UnexpectedLexemeException(_tokens.Peek(), [TokenType.Const, TokenType.Let, TokenType.Var]),
        };
    }

    private ConstDeclaration ParseConstDeclaration()
    {
        Expect(TokenType.Const);
        string name = ExpectIdentifier();
        Expect(TokenType.Colon);
        string typeName = ParseTypeName();
        Expect(TokenType.Assign);
        Expression initializer = ParseExpression();
        Expect(TokenType.Semicolon);
        return new ConstDeclaration(name, typeName, initializer);
    }

    private VariableDeclaration ParseVariableDeclaration()
    {
        TokenType declarationToken = _tokens.Peek().Type;
        if (declarationToken != TokenType.Let && declarationToken != TokenType.Var)
        {
            throw new UnexpectedLexemeException(_tokens.Peek(), [TokenType.Let, TokenType.Var]);
        }

        _tokens.Advance();
        string name = ExpectIdentifier();
        Expect(TokenType.Colon);
        string typeName = ParseTypeName();

        Expression initializer;
        if (Match(TokenType.Assign))
        {
            initializer = ParseExpression();
        }
        else
        {
            initializer = CreateDefaultInitializer(typeName);
        }

        Expect(TokenType.Semicolon);
        bool isMutable = declarationToken == TokenType.Var;
        return new VariableDeclaration(name, typeName, initializer, isMutable);
    }

    private Expression ParseStatementExpression()
    {
        if (Match(TokenType.OpenBrace))
        {
            return ParseScopeBody();
        }

        if (Is(TokenType.Print))
        {
            PrintStatement print = ParsePrintStatement();
            Expect(TokenType.Semicolon);
            return print;
        }

        if (Is(TokenType.Input))
        {
            InputStatement input = ParseInputStatement();
            Expect(TokenType.Semicolon);
            return input;
        }

        Expression expression = ParseExpression();
        Expect(TokenType.Semicolon);
        return expression;
    }

    private PrintStatement ParsePrintStatement()
    {
        Expect(TokenType.Print);
        Expect(TokenType.OpenParenthesis);
        Expression argument = ParseExpression();
        Expect(TokenType.CloseParenthesis);
        return new PrintStatement(argument);
    }

    private InputStatement ParseInputStatement()
    {
        Expect(TokenType.Input);
        Expect(TokenType.OpenParenthesis);
        string name = ExpectIdentifier();
        Expect(TokenType.CloseParenthesis);
        return new InputStatement(new IdentifierExpression(name));
    }

    private ScopeExpression ParseScopeBody()
    {
        List<ScopeItem> members = [];

        while (!Is(TokenType.CloseBrace))
        {
            if (Is(TokenType.EndOfFile))
            {
                throw new UnexpectedLexemeException(_tokens.Peek(), TokenType.CloseBrace);
            }

            if (IsDeclarationStart(_tokens.Peek().Type))
            {
                members.Add(new DeclarationScopeItem(ParseDeclaration()));
            }
            else
            {
                members.Add(new StatementScopeItem(ParseStatementExpression()));
            }
        }

        Expect(TokenType.CloseBrace);
        return new ScopeExpression(members);
    }

    private Expression ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    private Expression ParseAssignmentExpression()
    {
        Expression left = ParseTernaryExpression();
        if (Match(TokenType.Assign))
        {
            Expression right = ParseAssignmentExpression();
            return new AssignmentExpression(left, right);
        }

        return left;
    }

    // Цепочка заглушек для метода ParseAssignmentExpression(чтобы соответсовало EBNF)
    private Expression ParseTernaryExpression() => ParseLogicalOrExpression();

    private Expression ParseLogicalOrExpression() => ParseLogicalAndExpression();

    private Expression ParseLogicalAndExpression() => ParseEqualityExpression();

    private Expression ParseEqualityExpression() => ParseRelationalExpression();

    private Expression ParseRelationalExpression() => ParseAdditiveExpression();

    private Expression ParseAdditiveExpression()
    {
        Expression expression = ParseMultiplicativeExpression();
        while (Is(TokenType.Plus) || Is(TokenType.Minus))
        {
            TokenType op = _tokens.Peek().Type;
            _tokens.Advance();
            Expression right = ParseMultiplicativeExpression();
            expression = new BinaryExpression(expression, MapAdditiveOperator(op), right);
        }

        return expression;
    }

    private Expression ParseMultiplicativeExpression()
    {
        Expression expression = ParsePowerExpression();
        while (Is(TokenType.Multiply) || Is(TokenType.Divide) || Is(TokenType.Percent))
        {
            TokenType op = _tokens.Peek().Type;
            _tokens.Advance();
            Expression right = ParsePowerExpression();
            expression = new BinaryExpression(expression, MapMultiplicativeOperator(op), right);
        }

        return expression;
    }

    private Expression ParsePowerExpression()
    {
        Expression left = ParseUnaryExpression();
        if (Match(TokenType.Power))
        {
            Expression right = ParsePowerExpression();
            return new BinaryExpression(left, BinaryOperator.Power, right);
        }

        return left;
    }

    private Expression ParseUnaryExpression()
    {
        if (Match(TokenType.Plus))
        {
            return new UnaryExpression(UnaryOperator.Plus, ParseUnaryExpression());
        }

        if (Match(TokenType.Minus))
        {
            return new UnaryExpression(UnaryOperator.Minus, ParseUnaryExpression());
        }

        return ParsePrimaryExpression();
    }

    private Expression ParsePrimaryExpression()
    {
        Token token = _tokens.Peek();
        EnsureNotError(token);

        switch (token.Type)
        {
            case TokenType.IntLiteral:
                _tokens.Advance();
                return new LiteralExpression(Builtins.Int, token.Value?.ToString() ?? "0");
            case TokenType.FloatLiteral:
                _tokens.Advance();
                return new LiteralExpression(Builtins.Float, token.Value?.ToString() ?? "0");
            case TokenType.StringLiteral:
                _tokens.Advance();
                return new LiteralExpression(Builtins.String, token.Value?.ToString() ?? string.Empty);
            case TokenType.OpenParenthesis:
                _tokens.Advance();
                Expression expression = ParseExpression();
                Expect(TokenType.CloseParenthesis);
                return expression;
            case TokenType.Identifier:
                return ParseNameExpression();
            default:
                throw new UnexpectedLexemeException(
                    token,
                    [
                        TokenType.IntLiteral,
                        TokenType.FloatLiteral,
                        TokenType.StringLiteral,
                        TokenType.Identifier,
                        TokenType.OpenParenthesis,
                    ]
                );
        }
    }

    private Expression ParseNameExpression()
    {
        Token nameToken = _tokens.Peek();
        _tokens.Advance();
        if (nameToken.Type != TokenType.Identifier || nameToken.Value == null)
        {
            throw new UnexpectedLexemeException(nameToken, TokenType.Identifier);
        }

        string name = nameToken.Value.ToString();

        if (!Match(TokenType.OpenParenthesis))
        {
            return new IdentifierExpression(name);
        }

        List<Expression> arguments = ParseArgumentList();
        Expect(TokenType.CloseParenthesis);
        return new FunctionCallExpression(name, arguments);
    }

    private List<Expression> ParseArgumentList()
    {
        List<Expression> arguments = [];
        if (Is(TokenType.CloseParenthesis))
        {
            return arguments;
        }

        arguments.Add(ParseExpression());
        while (Match(TokenType.Comma))
        {
            arguments.Add(ParseExpression());
        }

        return arguments;
    }

    private string ParseTypeName()
    {
        Token token = _tokens.Peek();
        EnsureNotError(token);
        return token.Type switch
        {
            TokenType.TypeInt => AdvanceAndReturn(Builtins.Int),
            TokenType.TypeFloat => AdvanceAndReturn(Builtins.Float),
            TokenType.TypeString => AdvanceAndReturn(Builtins.String),
            TokenType.TypeVoid => AdvanceAndReturn(Builtins.Void),
            _ => throw new UnexpectedLexemeException(token, [TokenType.TypeInt, TokenType.TypeFloat, TokenType.TypeString, TokenType.TypeVoid]),
        };
    }

    private Expression CreateDefaultInitializer(string typeName)
    {
        return typeName switch
        {
            var t when t == Builtins.Int => new LiteralExpression(Builtins.Int, "0"),
            var t when t == Builtins.Float => new LiteralExpression(Builtins.Float, "0.0"),
            var t when t == Builtins.String => new LiteralExpression(Builtins.String, string.Empty),
            _ => throw new InvalidOperationException($"Неподдерживаемый тип объявления '{typeName}'."),
        };
    }

    private string ExpectIdentifier()
    {
        Token token = _tokens.Peek();
        EnsureNotError(token);
        if (token.Type != TokenType.Identifier || token.Value == null)
        {
            throw new UnexpectedLexemeException(token, TokenType.Identifier);
        }

        _tokens.Advance();
        return token.Value.ToString();
    }

    private void Expect(TokenType type)
    {
        Token token = _tokens.Peek();
        EnsureNotError(token);
        if (token.Type != type)
        {
            throw new UnexpectedLexemeException(token, type);
        }

        _tokens.Advance();
    }

    private bool Match(TokenType type)
    {
        if (!Is(type))
        {
            return false;
        }

        _tokens.Advance();
        return true;
    }

    private bool Is(TokenType type)
    {
        return _tokens.Peek().Type == type;
    }

    private string AdvanceAndReturn(string value)
    {
        _tokens.Advance();
        return value;
    }

    private void EnsureNotError(Token token)
    {
        if (token.Type == TokenType.Error)
        {
            throw new InvalidOperationException($"Ошибка лексера: {token.Value}");
        }
    }

    private static bool IsDeclarationStart(TokenType type)
    {
        return type is TokenType.Const or TokenType.Let or TokenType.Var;
    }

    private static BinaryOperator MapAdditiveOperator(TokenType type)
    {
        return type switch
        {
            TokenType.Plus => BinaryOperator.Add,
            TokenType.Minus => BinaryOperator.Subtract,
            _ => throw new InvalidOperationException($"Неподдерживаемый оператор сложения/вычитания {type}."),
        };
    }

    private static BinaryOperator MapMultiplicativeOperator(TokenType type)
    {
        return type switch
        {
            TokenType.Multiply => BinaryOperator.Multiply,
            TokenType.Divide => BinaryOperator.Divide,
            TokenType.Percent => BinaryOperator.Modulo,
            _ => throw new InvalidOperationException($"Неподдерживаемый оператор умножения/деления {type}."),
        };
    }
}
