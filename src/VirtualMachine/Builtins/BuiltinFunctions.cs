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
            case BuiltinFunctionCode.StringLength:
                StringLength(stack);
                break;
            case BuiltinFunctionCode.StringSubstring:
                StringSubstring(stack);
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

    private static void StringLength(Stack<Value> stack)
    {
        string s = stack.Pop().AsString();
        stack.Push(new Value(s.Length));
    }

    private static void StringSubstring(Stack<Value> stack)
    {
        int count = stack.Pop().AsInt();
        int start = stack.Pop().AsInt();
        string s = stack.Pop().AsString();
        stack.Push(new Value(s.Substring(start, count)));
    }
}
