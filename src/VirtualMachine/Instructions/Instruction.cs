using TLPMinion.Runtime;

namespace TLPMinion.VirtualMachine.Instructions;

public sealed class Instruction
{
    public Instruction(InstructionCode code)
    {
        Code = code;
        Operand = Value.Void;
    }

    public Instruction(InstructionCode code, Value operand)
    {
        Code = code;
        Operand = operand;
    }

    public Instruction(InstructionCode code, int operand)
        : this(code, new Value(operand))
    {
    }

    public Instruction(InstructionCode code, string operand)
        : this(code, new Value(operand))
    {
    }

    public InstructionCode Code { get; }

    public Value Operand { get; }
}
