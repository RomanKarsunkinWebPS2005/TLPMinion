using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Ast.Statements;

namespace TLPMinion.Ast;

public interface IAstVisitor
{
    void Visit(ConstDeclaration declaration);

    void Visit(VariableDeclaration declaration);

    void Visit(AssignmentExpression expression);

    void Visit(BinaryExpression expression);

    void Visit(ConditionalExpression expression);

    void Visit(FunctionCallExpression expression);

    void Visit(IdentifierExpression expression);

    void Visit(InputStatement statement);

    void Visit(LiteralExpression expression);

    void Visit(PrintStatement statement);

    void Visit(ScopeExpression expression);

    void Visit(UnaryExpression expression);
}
