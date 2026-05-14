using TLPMinion.Ast.Expressions;
using TLPMinion.Semantics.Passes;
using TLPMinion.Semantics.Symbols;

namespace TLPMinion.Semantics;

public sealed class SemanticsChecker
{
    private readonly AbstractPass[] _passes;

    public SemanticsChecker()
    {
        SymbolsTable globalSymbols = new(parent: null);
        _passes =
        [
            new ResolveNamesPass(globalSymbols),
            new TypeCheckingPass(),
            new CompileTimeConstantPass(),
        ];
    }

    public void Check(Expression program)
    {
        foreach (AbstractPass pass in _passes)
        {
            program.Accept(pass);
        }
    }
}
