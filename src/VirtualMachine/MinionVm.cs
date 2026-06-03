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
                case InstructionCode.Equal:
                    _stack.Push(CompareEqual(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.NotEqual:
                    _stack.Push(CompareNotEqual(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Less:
                    _stack.Push(CompareLess(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.LessOrEqual:
                    _stack.Push(CompareLessOrEqual(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Not:
                    _stack.Push(LogicalNot(_stack.Pop()));
                    break;
                case InstructionCode.And:
                    _stack.Push(LogicalAnd(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Or:
                    _stack.Push(LogicalOr(_stack.Pop(), _stack.Pop()));
                    break;
                case InstructionCode.Jump:
                    _instructionPointer = instruction.Operand.AsInt();
                    break;
                case InstructionCode.JumpIfTrue:
                    {
                        bool condition = _stack.Pop().AsBool();
                        if (condition)
                        {
                            _instructionPointer = instruction.Operand.AsInt();
                        }
                    }

                    break;
                case InstructionCode.JumpIfFalse:
                    {
                        bool condition = _stack.Pop().AsBool();
                        if (!condition)
                        {
                            _instructionPointer = instruction.Operand.AsInt();
                        }
                    }

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
        if (left.IsString() && right.IsString())
        {
            return new Value(left.AsString() + right.AsString());
        }

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

    private static Value CompareEqual(Value right, Value left)
    {
        EnsureSameComparisonOperands(left, right);
        return new Value(left.Equals(right));
    }

    private static Value CompareNotEqual(Value right, Value left)
    {
        EnsureSameComparisonOperands(left, right);
        return new Value(!left.Equals(right));
    }

    private static Value CompareLess(Value right, Value left)
    {
        return new Value(CompareOrdering(left, right) < 0);
    }

    private static Value CompareLessOrEqual(Value right, Value left)
    {
        return new Value(CompareOrdering(left, right) <= 0);
    }

    private static Value LogicalNot(Value value)
    {
        return new Value(!value.AsBool());
    }

    private static Value LogicalAnd(Value right, Value left)
    {
        return new Value(left.AsBool() && right.AsBool());
    }

    private static Value LogicalOr(Value right, Value left)
    {
        return new Value(left.AsBool() || right.AsBool());
    }

    private static void EnsureSameComparisonOperands(Value left, Value right)
    {
        if ((left.IsInt() && right.IsInt())
            || (left.IsDouble() && right.IsDouble())
            || (left.IsString() && right.IsString())
            || (left.IsBool() && right.IsBool()))
        {
            return;
        }

        throw new InvalidOperationException("Операнды сравнения должны быть одного типа.");
    }

    private static int CompareOrdering(Value left, Value right)
    {
        if (left.IsInt() && right.IsInt())
        {
            return left.AsInt().CompareTo(right.AsInt());
        }

        if (left.IsDouble() && right.IsDouble())
        {
            return left.AsDouble().CompareTo(right.AsDouble());
        }

        if (left.IsString() && right.IsString())
        {
            return string.CompareOrdinal(left.AsString(), right.AsString());
        }

        throw new InvalidOperationException("Операнды упорядочивания должны быть одного типа (Int, Float или String).");
    }

    private static void ValidateProgram(IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0 || instructions[^1].Code != InstructionCode.Halt)
        {
            throw new InvalidOperationException("Программа должна заканчиваться инструкцией Halt.");
        }
    }
}
