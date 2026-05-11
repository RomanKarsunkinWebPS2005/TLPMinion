using TLPMinion.Ast.Declarations;

namespace TLPMinion.Semantics.Symbols;

public sealed class SymbolsTable
{
    private readonly SymbolsTable? _parent;
    private readonly Dictionary<string, AbstractVariableDeclaration> _variables = [];

    public SymbolsTable(SymbolsTable? parent)
    {
        _parent = parent;
    }

    public SymbolsTable? Parent => _parent;

    public void DeclareVariable(AbstractVariableDeclaration declaration)
    {
        if (!_variables.TryAdd(declaration.Name, declaration))
        {
            throw new InvalidOperationException(
                $"Повторное объявление '{declaration.Name}' в той же области видимости.");
        }
    }

    public AbstractVariableDeclaration GetVariable(string name)
    {
        if (_variables.TryGetValue(name, out AbstractVariableDeclaration? declaration))
        {
            return declaration;
        }

        if (_parent != null)
        {
            return _parent.GetVariable(name);
        }

        throw new InvalidOperationException($"Неизвестный идентификатор '{name}'.");
    }
}
