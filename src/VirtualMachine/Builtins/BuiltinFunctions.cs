using System.Globalization;

using TLPMinion.Runtime;

namespace TLPMinion.VirtualMachine.Builtins;

public sealed class BuiltinFunctions
{
    private readonly IEnvironment _environment;

    public BuiltinFunctions(IEnvironment environment)
    {
        _environment = environment;
    }

    public void Invoke(BuiltinFunctionCode code, Stack<Value> stack)
    {
        switch (code)
        {
            case BuiltinFunctionCode.Print:
                Print(stack.Pop());
                break;
            case BuiltinFunctionCode.InputInt:
                InputInt(stack);
                break;
            case BuiltinFunctionCode.InputFloat:
                InputFloat(stack);
                break;
            case BuiltinFunctionCode.InputString:
                InputString(stack);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(code), code, null);
        }
    }

    private void Print(Value value)
    {
        _environment.Print(value.ToDisplayString());
    }

    private void InputInt(Stack<Value> stack)
    {
        string word = _environment.ReadWord();
        if (!int.TryParse(word, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
        {
            throw new FormatException($"Некорректное целое: '{word}'");
        }

        stack.Push(new Value(parsed));
    }

    private void InputFloat(Stack<Value> stack)
    {
        string word = _environment.ReadWord();
        if (!double.TryParse(word, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
        {
            throw new FormatException($"Некорректное вещественное: '{word}'");
        }

        stack.Push(new Value(parsed));
    }

    private void InputString(Stack<Value> stack)
    {
        string word = _environment.ReadWord();
        stack.Push(new Value(word));
    }
}
