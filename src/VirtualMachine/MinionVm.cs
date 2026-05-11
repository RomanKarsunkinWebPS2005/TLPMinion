using TLPMinion.Runtime;
using TLPMinion.VirtualMachine.Builtins;
using TLPMinion.VirtualMachine.Instructions;

namespace TLPMinion.VirtualMachine;

public sealed class MinionVm
{
    private readonly BuiltinFunctions _builtins;
    private readonly IReadOnlyList<Instruction> _instructions;
    private readonly Stack<Value> _stack = new();
    private int _instructionPointer;
    private int _exitCode;
    private VariablesTable? _variables;
    private Value _result = Value.Void;

    public MinionVm(IEnvironment environment, IReadOnlyList<Instruction> instructions)
    {
        ValidateProgram(instructions);
        _builtins = new BuiltinFunctions(environment);
        _instructions = instructions;
        _variables = new VariablesTable(parent: null);
    }

    public int ExitCode => _exitCode;

    public Value RunProgram()
    {
        while (true)
        {
            Instruction instruction = _instructions[_instructionPointer++];
            switch (instruction.Code)
            {
                case InstructionCode.Push:
                    _stack.Push(instruction.Operand);
                    break;
                case InstructionCode.Pop:
                    _stack.Pop();
                    break;
                case InstructionCode.DefineVar:
                    {
                        Value value = _stack.Pop();
                        string name = instruction.Operand.AsString();
                        _variables!.DefineVariable(name, value);
                    }

                    break;
                case InstructionCode.StoreVar:
                    {
                        Value value = _stack.Pop();
                        string name = instruction.Operand.AsString();
                        _variables!.AssignVariable(name, value);
                    }

                    break;
                case InstructionCode.LoadVar:
                    {
                        string name = instruction.Operand.AsString();
                        _stack.Push(_variables!.GetVariable(name));
                    }

                    break;
                case InstructionCode.Add:
                    _stack.Push(Add(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Subtract:
                    _stack.Push(Subtract(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Multiply:
                    _stack.Push(Multiply(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Divide:
                    _stack.Push(Divide(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Modulo:
                    _stack.Push(Modulo(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Power:
                    _stack.Push(Power(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Negate:
                    _stack.Push(Negate(_stack.Pop()));
                    break;
                case InstructionCode.CallBuiltin:
                    _builtins.Invoke((BuiltinFunctionCode)instruction.Operand.AsInt(), _stack);
                    break;
                case InstructionCode.StoreResult:
                    _result = _stack.Pop();
                    break;
                case InstructionCode.PushVars:
                    {
                        int depth = instruction.Operand.AsInt();
                        VariablesTable? parent = depth == 0
                            ? null
                            : _variables!.GetAncestor(depth);
                        _variables = new VariablesTable(parent);
                    }

                    break;
                case InstructionCode.PopVars:
                    _variables = _variables!.Parent;
                    break;
                case InstructionCode.Halt:
                    _exitCode = _stack.Pop().AsInt();
                    return _result;
                default:
                    throw new NotImplementedException(instruction.Code.ToString());
            }
        }
    }

    private static Value Negate(Value value)
    {
        if (value.IsInt())
        {
            return new Value(-value.AsInt());
        }

        return new Value(-value.AsDouble());
    }

    private static Value Add(Value right, Value left)
    {
        if (left.IsInt() && right.IsInt())
        {
            return new Value(left.AsInt() + right.AsInt());
        }

        return new Value(left.AsDouble() + right.AsDouble());
    }

    private static Value Subtract(Value right, Value left)
    {
        if (left.IsInt() && right.IsInt())
        {
            return new Value(left.AsInt() - right.AsInt());
        }

        return new Value(left.AsDouble() - right.AsDouble());
    }

    private static Value Multiply(Value right, Value left)
    {
        if (left.IsInt() && right.IsInt())
        {
            return new Value(left.AsInt() * right.AsInt());
        }

        return new Value(left.AsDouble() * right.AsDouble());
    }

    private static Value Divide(Value right, Value left)
    {
        if (left.IsInt() && right.IsInt())
        {
            return new Value(left.AsInt() / right.AsInt());
        }

        return new Value(left.AsDouble() / right.AsDouble());
    }

    private static Value Modulo(Value right, Value left)
    {
        return new Value(left.AsInt() % right.AsInt());
    }

    private static Value Power(Value right, Value left)
    {
        double value = Math.Pow(left.AsDouble(), right.AsDouble());
        return new Value(value);
    }

    private static void ValidateProgram(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0 || instructions[^1].Code != InstructionCode.Halt)
        {
            throw new InvalidOperationException("Программа должна заканчиваться инструкцией Halt.");
        }
    }
}
