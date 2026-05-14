using TLPMinion.Runtime;
using TLPMinion.Tests.TestLibrary.TestDoubles;
using TLPMinion.VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public sealed class HaltTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetHaltVmData))]
    public void Can_halt_VM(int exitCode)
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.Push, exitCode),
            new Instruction(InstructionCode.Halt),
        ]);
        Value result = vm.RunProgram();

        Assert.Equal(exitCode, vm.ExitCode);
        Assert.Equal(Value.Void, result);
        Assert.Empty(environment.Output);
    }

    public static TheoryData<int> GetHaltVmData()
    {
        return
        [
            0,
            1,
        ];
    }
}
