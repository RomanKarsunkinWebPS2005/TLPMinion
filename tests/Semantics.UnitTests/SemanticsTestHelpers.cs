using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Parser;
using TLPMinion.Semantics;

namespace Semantics.UnitTests;

internal static class SemanticsTestHelpers
{
    public static ScopeExpression ParseProgram(string source)
    {
        Parser parser = new(source);
        return (ScopeExpression)parser.ParseProgram();
    }

    public static void RunSemantics(Expression program)
    {
        SemanticsChecker checker = new();
        checker.Check(program);
    }

    public static ConstDeclaration RequireConst(ScopeExpression scope, string name)
    {
        foreach (ScopeItem item in scope.Members)
        {
            if (item is DeclarationScopeItem { Declaration: ConstDeclaration c } && c.Name == name)
            {
                return c;
            }
        }

        throw new InvalidOperationException($"Не найдено объявление const '{name}'.");
    }
}
