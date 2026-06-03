using TLPMinion.Runtime;
using TLPMinion.Tests.TestLibrary.TestDoubles;
using TLPMinion.VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public sealed class BooleanEvaluationTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetComparisonData))]
    public void Can_compare_values(InstructionCode code, Value left, Value right, bool expected)
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, left),
            new Instruction(InstructionCode.Push, right),
            new Instruction(code),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Value result = vm.RunProgram();

        Assert.Equal(new Value(expected), result);
    }

    public static TheoryData<InstructionCode, Value, Value, bool> GetComparisonData()
    {
        return new TheoryData<InstructionCode, Value, Value, bool>
        {
            { InstructionCode.Equal, new Value(1), new Value(1), true },
            { InstructionCode.Equal, new Value(1), new Value(2), false },
            { InstructionCode.NotEqual, new Value(1), new Value(2), true },
            { InstructionCode.Less, new Value(1), new Value(2), true },
            { InstructionCode.LessOrEqual, new Value(2), new Value(2), true },
            { InstructionCode.Equal, new Value(true), new Value(false), false },
            { InstructionCode.Equal, new Value("a"), new Value("a"), true },
            { InstructionCode.Less, new Value("a"), new Value("b"), true },
            { InstructionCode.LessOrEqual, new Value("a"), new Value("a"), true },
            { InstructionCode.Equal, new Value(1.0), new Value(1.0), true },
            { InstructionCode.Less, new Value(1.0), new Value(2.0), true },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetLogicalData))]
    public void Can_evaluate_logical_operators(InstructionCode code, bool left, bool right, bool expected)
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(left)),
            new Instruction(InstructionCode.Push, new Value(right)),
            new Instruction(code),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Value result = vm.RunProgram();

        Assert.Equal(new Value(expected), result);
    }

    public static TheoryData<InstructionCode, bool, bool, bool> GetLogicalData()
    {
        return new TheoryData<InstructionCode, bool, bool, bool>
        {
            { InstructionCode.And, true, true, true },
            { InstructionCode.And, true, false, false },
            { InstructionCode.Or, false, true, true },
            { InstructionCode.Or, false, false, false },
        };
    }

    [Fact]
    public void Greater_via_swapped_operands_uses_less()
    {
        // a > b  →  push b, push a, Less
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(2)),
            new Instruction(InstructionCode.Push, new Value(3)),
            new Instruction(InstructionCode.Less),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Assert.Equal(new Value(true), vm.RunProgram());
    }

    [Fact]
    public void String_greater_via_swapped_operands_uses_less()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value("a")),
            new Instruction(InstructionCode.Push, new Value("ab")),
            new Instruction(InstructionCode.Less),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Assert.Equal(new Value(true), vm.RunProgram());
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetStringOrderingData))]
    public void Can_compare_strings_ordinally(InstructionCode code, string left, string right, bool expected)
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(left)),
            new Instruction(InstructionCode.Push, new Value(right)),
            new Instruction(code),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Assert.Equal(new Value(expected), vm.RunProgram());
    }

    public static TheoryData<InstructionCode, string, string, bool> GetStringOrderingData()
    {
        return new TheoryData<InstructionCode, string, string, bool>
        {
            { InstructionCode.Less, "A", "a", true },
            { InstructionCode.Less, "ab", "b", true },
            { InstructionCode.LessOrEqual, "a", "a", true },
            { InstructionCode.Less, "a", "🙂", true },
            { InstructionCode.Less, "а", "я", true },
        };
    }

    [Fact]
    public void Can_apply_logical_not()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(true)),
            new Instruction(InstructionCode.Not),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Assert.Equal(new Value(false), vm.RunProgram());
    }

    [Fact]
    public void Jump_skips_instructions()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, 10),
            new Instruction(InstructionCode.Jump, 4),
            new Instruction(InstructionCode.Push, 99),
            new Instruction(InstructionCode.Pop),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Assert.Equal(new Value(10), vm.RunProgram());
    }

    [Fact]
    public void JumpIfFalse_selects_false_branch()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(false)),
            new Instruction(InstructionCode.JumpIfFalse, 5),
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Jump, 7),
            new Instruction(InstructionCode.Push, 2),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Assert.Equal(new Value(2), vm.RunProgram());
    }

    [Fact]
    public void JumpIfTrue_selects_true_branch()
    {
        List<Instruction> program =
        [
            new Instruction(InstructionCode.Push, new Value(true)),
            new Instruction(InstructionCode.JumpIfTrue, 5),
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Jump, 7),
            new Instruction(InstructionCode.Push, 2),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ];

        MinionVm vm = new(new FakeEnvironment(), program);
        Assert.Equal(new Value(2), vm.RunProgram());
    }
}
