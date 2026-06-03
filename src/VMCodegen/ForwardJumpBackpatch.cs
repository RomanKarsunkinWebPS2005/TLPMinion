namespace TLPMinion.VMCodegen;

/// <summary>
/// Инструкция jump с временным операндом,
/// который заполняется <see cref="InstructionsBuilder.BackpatchForwardJump"/> после создания целевого блока.
/// </summary>
public readonly struct ForwardJumpBackpatch
{
    public ForwardJumpBackpatch(BasicBlock block, int instructionIndex)
    {
        Block = block;
        InstructionIndex = instructionIndex;
    }

    public BasicBlock Block { get; }

    public int InstructionIndex { get; }
}
