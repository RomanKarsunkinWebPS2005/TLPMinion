using TLPMinion.Runtime;
using TLPMinion.Tests.TestLibrary.TestDoubles;
using TLPMinion.VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public sealed class EvaluationTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetEvaluateExpressionData))]
    public void Can_evaluate_expression(List<Instruction> program, Value expected)
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, program);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expected, result);
        Assert.Empty(environment.Output);
    }

    public static TheoryData<List<Instruction>, Value> GetEvaluateExpressionData()
    {
        return new TheoryData<List<Instruction>, Value>
        {
            {
                [
                    new Instruction(InstructionCode.Push, 67),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(67)
            },
            {
                [
                    new Instruction(InstructionCode.Push, new Value(1.5)),
                    new Instruction(InstructionCode.Push, new Value(2.5)),
                    new Instruction(InstructionCode.Add),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(4.0)
            },
            {
                [
                    new Instruction(InstructionCode.Push, 20),
                    new Instruction(InstructionCode.Push, 50),
                    new Instruction(InstructionCode.Add),
                    new Instruction(InstructionCode.Push, 3),
                    new Instruction(InstructionCode.Subtract),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(67)
            },
            {
                [
                    new Instruction(InstructionCode.Push, 20),
                    new Instruction(InstructionCode.Push, 50),
                    new Instruction(InstructionCode.Multiply),
                    new Instruction(InstructionCode.Push, -5),
                    new Instruction(InstructionCode.Divide),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(-200)
            },
            {
                [
                    new Instruction(InstructionCode.Push, 10),
                    new Instruction(InstructionCode.Push, 3),
                    new Instruction(InstructionCode.Modulo),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(1)
            },
            {
                [
                    new Instruction(InstructionCode.Push, new Value(2.0)),
                    new Instruction(InstructionCode.Push, new Value(3.0)),
                    new Instruction(InstructionCode.Power),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(8.0)
            },
            {
                [
                    new Instruction(InstructionCode.Push, 1024),
                    new Instruction(InstructionCode.Negate),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(-1024)
            },
            {
                [
                    new Instruction(InstructionCode.Push, 1024),
                    new Instruction(InstructionCode.Push, 702),
                    new Instruction(InstructionCode.Pop),
                    new Instruction(InstructionCode.Negate),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(-1024)
            },
            {
                [
                    new Instruction(InstructionCode.Push, new Value(1.25)),
                    new Instruction(InstructionCode.Negate),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value(-1.25)
            },
            {
                [
                    new Instruction(InstructionCode.Push, new Value("left")),
                    new Instruction(InstructionCode.Push, new Value("right")),
                    new Instruction(InstructionCode.Add),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                new Value("leftright")
            },
        };
    }

    [Fact]
    public void Integer_division_by_zero_throws()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Divide),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<DivideByZeroException>(() =>
        {
            _ = vm.RunProgram();
        });
    }
}
