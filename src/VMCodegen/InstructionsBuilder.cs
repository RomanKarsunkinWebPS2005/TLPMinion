using TLPMinion.VirtualMachine.Instructions;

namespace TLPMinion.VMCodegen;

public sealed class InstructionsBuilder
{
    private readonly List<BasicBlock> _basicBlocks;
    private BasicBlock _insertPoint;

    public InstructionsBuilder()
    {
        _basicBlocks = [];
        _insertPoint = CreateBasicBlock();
    }

    public BasicBlock InsertPoint
    {
        get => _insertPoint;

        set
        {
            if (!ReferenceEquals(_basicBlocks[value.Id], value))
            {
                throw new InvalidOperationException("Базовый блок не принадлежит текущему построителю инструкций.");
            }

            _insertPoint = value;
        }
    }

    public List<Instruction> Finish()
    {
        List<int> addresses = CalculateBasicBlockAddresses();
        List<Instruction> instructions = [];

        foreach (BasicBlock block in _basicBlocks)
        {
            foreach (Instruction instruction in block.Instructions)
            {
                if (IsJump(instruction.Code))
                {
                    int newAddress = addresses[instruction.Operand.AsInt()];
                    instructions.Add(new Instruction(instruction.Code, newAddress));
                }
                else
                {
                    instructions.Add(instruction);
                }
            }
        }

        return instructions;
    }

    public void Append(Instruction instruction)
    {
        if (IsJump(instruction.Code))
        {
            throw new InvalidOperationException($"Инструкцию {instruction.Code} нельзя добавить этим методом.");
        }

        _insertPoint.Append(instruction);
    }

    public void AppendJump(InstructionCode code, BasicBlock target)
    {
        if (!IsJump(code))
        {
            throw new InvalidOperationException($"Инструкция {code} не является переходом.");
        }

        _insertPoint.Append(new Instruction(code, target.Id));
    }

    public ForwardJumpBackpatch AppendForwardJump(InstructionCode code)
    {
        if (!IsJump(code))
        {
            throw new InvalidOperationException($"Инструкция {code} не является переходом.");
        }

        _insertPoint.Append(new Instruction(code, -1));
        return new ForwardJumpBackpatch(_insertPoint, _insertPoint.Instructions.Count - 1);
    }

    public void BackpatchForwardJump(ForwardJumpBackpatch patch, BasicBlock target)
    {
        if (!ReferenceEquals(_basicBlocks[patch.Block.Id], patch.Block))
        {
            throw new InvalidOperationException("Базовый блок не принадлежит текущему построителю инструкций.");
        }

        Instruction instruction = patch.Block.Instructions[patch.InstructionIndex];
        patch.Block.Instructions[patch.InstructionIndex] = new Instruction(instruction.Code, target.Id);
    }

    public BasicBlock CreateBasicBlock()
    {
        BasicBlock block = new(_basicBlocks.Count);
        _basicBlocks.Add(block);
        return block;
    }

    private static bool IsJump(InstructionCode code)
    {
        return code is InstructionCode.Jump or InstructionCode.JumpIfFalse or InstructionCode.JumpIfTrue;
    }

    private List<int> CalculateBasicBlockAddresses()
    {
        List<int> basicBlockAddresses = new(capacity: _basicBlocks.Count);
        int nextBlockAddress = 0;
        foreach (BasicBlock block in _basicBlocks)
        {
            basicBlockAddresses.Add(nextBlockAddress);
            nextBlockAddress += block.Instructions.Count;
        }

        return basicBlockAddresses;
    }
}
