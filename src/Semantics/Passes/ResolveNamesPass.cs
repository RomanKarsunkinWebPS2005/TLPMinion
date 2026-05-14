using TLPMinion.Ast.Declarations;
using TLPMinion.Ast.Expressions;
using TLPMinion.Ast.Statements;
using TLPMinion.Semantics.Symbols;

namespace TLPMinion.Semantics.Passes;

public sealed class ResolveNamesPass : AbstractPass
{
    private SymbolsTable _symbols;

    public ResolveNamesPass(SymbolsTable globalSymbols)
    {
        _symbols = globalSymbols;
    }

    public override void Visit(ScopeExpression expression)
    {
        _symbols = new SymbolsTable(_symbols);
        try
        {
            base.Visit(expression);
        }
        finally
        {
            _symbols = _symbols.Parent!;
        }
    }

    public override void Visit(ConstDeclaration declaration)
    {
        declaration.Initializer.Accept(this);
        _symbols.DeclareVariable(declaration);
    }

    public override void Visit(VariableDeclaration declaration)
    {
        declaration.Initializer.Accept(this);
        _symbols.DeclareVariable(declaration);
    }

    public override void Visit(IdentifierExpression expression)
    {
        expression.Variable = _symbols.GetVariable(expression.Name);
    }

    public override void Visit(InputStatement statement)
    {
        statement.Target.Accept(this);
    }

    public override void Visit(PrintStatement statement)
    {
        statement.Argument.Accept(this);
    }
}
