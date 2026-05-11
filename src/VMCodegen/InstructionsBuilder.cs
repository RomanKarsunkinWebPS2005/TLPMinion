using TLPMinion.VirtualMachine.Instructions;

namespace TLPMinion.VMCodegen;

public sealed class InstructionsBuilder
{
    private readonly List<Instruction> _list = [];

    public void Append(Instruction instruction)
    {
        _list.Add(instruction);
    }

    public IReadOnlyList<Instruction> Finish()
    {
        return _list;
    }
}
