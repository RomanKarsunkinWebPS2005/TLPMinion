using TLPMinion.Runtime;
using TLPMinion.Tests.TestLibrary.TestDoubles;
using TLPMinion.VirtualMachine.Builtins;
using TLPMinion.VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public sealed class VariablesTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetStoreLoadAndPrintData))]
    public void Can_define_variables_print_and_use_stack(
        List<Instruction> program,
        string expectedOutput)
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, program);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(Value.Void, result);
        Assert.Equal(expectedOutput, environment.Output);
    }

    public static TheoryData<List<Instruction>, string> GetStoreLoadAndPrintData()
    {
        return new TheoryData<List<Instruction>, string>
        {
            {
                [
                    new Instruction(InstructionCode.PushVars, 0),
                    new Instruction(InstructionCode.Push, 10),
                    new Instruction(InstructionCode.DefineVar, "x"),
                    new Instruction(InstructionCode.Push, 14),
                    new Instruction(InstructionCode.DefineVar, "y"),
                    new Instruction(InstructionCode.LoadVar, "x"),
                    new Instruction(InstructionCode.LoadVar, "x"),
                    new Instruction(InstructionCode.Multiply),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.PopVars),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "100"
            },
            {
                [
                    new Instruction(InstructionCode.PushVars, 0),
                    new Instruction(InstructionCode.Push, 10),
                    new Instruction(InstructionCode.DefineVar, "x"),
                    new Instruction(InstructionCode.PushVars, 1),
                    new Instruction(InstructionCode.Push, 12),
                    new Instruction(InstructionCode.DefineVar, "x"),
                    new Instruction(InstructionCode.LoadVar, "x"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.PopVars),
                    new Instruction(InstructionCode.Push, ", "),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.LoadVar, "x"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.PopVars),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "12, 10"
            },
            {
                [
                    new Instruction(InstructionCode.PushVars, 0),
                    new Instruction(InstructionCode.Push, new Value(3.14)),
                    new Instruction(InstructionCode.DefineVar, "f"),
                    new Instruction(InstructionCode.LoadVar, "f"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.PopVars),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "3.14"
            },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetAssignThroughInnerScopeData))]
    public void Can_assign_outer_variable_from_inner_scope_when_name_not_redeclared(
        List<Instruction> program,
        Value expected)
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, program);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expected, result);
        Assert.Empty(environment.Output);
    }

    public static TheoryData<List<Instruction>, Value> GetAssignThroughInnerScopeData()
    {
        return new TheoryData<List<Instruction>, Value>
        {
            {
                [
                    new Instruction(InstructionCode.PushVars, 0),
                    new Instruction(InstructionCode.Push, 5),
                    new Instruction(InstructionCode.DefineVar, "x"),
                    new Instruction(InstructionCode.PushVars, 1),
                    new Instruction(InstructionCode.LoadVar, "x"),
                    new Instruction(InstructionCode.Push, 7),
                    new Instruction(InstructionCode.StoreVar, "x"),
                    new Instruction(InstructionCode.PopVars),
                    new Instruction(InstructionCode.LoadVar, "x"),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.PopVars),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(7)
            },
        };
    }

    [Fact]
    public void Duplicate_define_in_same_scope_throws()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.PushVars, 0),
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.DefineVar, "x"),
            new Instruction(InstructionCode.Push, 2),
            new Instruction(InstructionCode.DefineVar, "x"),
            new Instruction(InstructionCode.PopVars),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = vm.RunProgram();
        });
    }

    [Fact]
    public void StoreVar_unknown_name_throws()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.PushVars, 0),
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.StoreVar, "nope"),
            new Instruction(InstructionCode.PopVars),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = vm.RunProgram();
        });
    }

    [Fact]
    public void LoadVar_after_pop_inner_scope_throws_for_inner_only_name()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.PushVars, 0),
            new Instruction(InstructionCode.PushVars, 1),
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.DefineVar, "innerOnly"),
            new Instruction(InstructionCode.PopVars),
            new Instruction(InstructionCode.LoadVar, "innerOnly"),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.PopVars),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = vm.RunProgram();
        });
    }
}
