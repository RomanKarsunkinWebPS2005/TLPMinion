using TLPMinion.Ast;
using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
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
        List<Declaration> declarations = [];
        List<Expression> expressions = [];

        while (!Is(TokenType.EndOfFile))
        {
            if (IsDeclarationStart(_tokens.Peek().Type))
            {
                declarations.Add(ParseDeclaration());
                continue;
            }

            expressions.Add(ParseStatementExpression());
        }

        return new ScopeExpression(declarations, expressions);
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
        return new VariableDeclaration(name, typeName, initializer);
    }

    private Expression ParseStatementExpression()
    {
        if (Match(TokenType.OpenBrace))
        {
            return ParseScopeBody();
        }

        Expression expression = ParseExpression();
        Expect(TokenType.Semicolon);
        return expression;
    }

    private ScopeExpression ParseScopeBody()
    {
        List<Declaration> declarations = [];
        List<Expression> expressions = [];

        while (!Is(TokenType.CloseBrace))
        {
            if (Is(TokenType.EndOfFile))
            {
                throw new UnexpectedLexemeException(_tokens.Peek(), TokenType.CloseBrace);
            }

            if (IsDeclarationStart(_tokens.Peek().Type))
            {
                declarations.Add(ParseDeclaration());
            }
            else
            {
                expressions.Add(ParseStatementExpression());
            }
        }

        Expect(TokenType.CloseBrace);
        return new ScopeExpression(declarations, expressions);
    }

    private Expression ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    private Expression ParseAssignmentExpression()
    {
        Expression left = ParseAdditiveExpression();
        if (Match(TokenType.Assign))
        {
            Expression right = ParseAssignmentExpression();
            return new AssignmentExpression(left, right);
        }

        return left;
    }

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
            case TokenType.OpenParenthesis:
                _tokens.Advance();
                Expression expression = ParseExpression();
                Expect(TokenType.CloseParenthesis);
                return expression;
            case TokenType.Identifier:
            case TokenType.Print:
            case TokenType.Input:
                return ParseNameExpression();
            default:
                throw new UnexpectedLexemeException(
                    token,
                    [
                        TokenType.IntLiteral,
                        TokenType.FloatLiteral,
                        TokenType.Identifier,
                        TokenType.Print,
                        TokenType.Input,
                        TokenType.OpenParenthesis,
                    ]
                );
        }
    }

    private Expression ParseNameExpression()
    {
        Token nameToken = _tokens.Peek();
        _tokens.Advance();
        string name = nameToken.Value?.ToString() ?? nameToken.Type switch
        {
            TokenType.Print => Builtins.Print,
            TokenType.Input => Builtins.Input,
            _ => string.Empty,
        };

        if (string.IsNullOrEmpty(name))
        {
            throw new UnexpectedLexemeException(nameToken, TokenType.Identifier);
        }

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
            _ => throw new UnexpectedLexemeException(token, [TokenType.TypeInt, TokenType.TypeFloat]),
        };
    }

    private Expression CreateDefaultInitializer(string typeName)
    {
        return typeName switch
        {
            var t when t == Builtins.Int => new LiteralExpression(Builtins.Int, "0"),
            var t when t == Builtins.Float => new LiteralExpression(Builtins.Float, "0.0"),
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
