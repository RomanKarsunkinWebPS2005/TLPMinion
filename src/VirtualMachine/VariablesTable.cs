using TLPMinion.Runtime;

namespace TLPMinion.VirtualMachine;

public sealed class VariablesTable
{
    private readonly VariablesTable? _parent;
    private readonly Dictionary<string, Value> _variables = [];
    private readonly int _depth;

    public VariablesTable(VariablesTable? parent = null)
    {
        _parent = parent;
        _depth = (parent?._depth ?? 0) + 1;
    }

    public VariablesTable? Parent => _parent;

    public Value GetVariable(string name)
    {
        if (_variables.TryGetValue(name, out Value? value))
        {
            return value;
        }

        if (_parent != null)
        {
            return _parent.GetVariable(name);
        }

        throw new InvalidOperationException($"Переменная '{name}' не найдена.");
    }

    public void DefineVariable(string name, Value value)
    {
        if (!_variables.TryAdd(name, value))
        {
            throw new InvalidOperationException($"Переменная '{name}' уже объявлена в этой области.");
        }
    }

    public void AssignVariable(string name, Value value)
    {
        if (!TryAssignVariable(name, value))
        {
            throw new InvalidOperationException($"Переменная '{name}' не найдена.");
        }
    }

    public VariablesTable GetAncestor(int depth)
    {
        if (depth <= 0)
        {
            throw new InvalidOperationException($"Некорректная глубина {depth}.");
        }

        if (depth > _depth)
        {
            throw new InvalidOperationException($"Запрошена глубина {depth}, текущая {_depth}.");
        }

        VariablesTable table = this;
        while (table._depth != depth)
        {
            table = table._parent!;
        }

        return table;
    }

    private bool TryAssignVariable(string name, Value value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            return true;
        }

        return _parent != null && _parent.TryAssignVariable(name, value);
    }
}
