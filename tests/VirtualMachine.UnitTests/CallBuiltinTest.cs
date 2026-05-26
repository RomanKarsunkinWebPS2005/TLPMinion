using TLPMinion.Runtime;
using TLPMinion.Tests.TestLibrary.TestDoubles;
using TLPMinion.VirtualMachine.Builtins;
using TLPMinion.VirtualMachine.Instructions;

namespace VirtualMachine.UnitTests;

public sealed class CallBuiltinTest
{
    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetPrintAndVoidResultData))]
    public void Can_print_and_finish_with_void_result(
        List<Instruction> program,
        string input,
        string expectedOutput)
    {
        FakeEnvironment environment = new();
        environment.AddInput(input);
        MinionVm vm = new(environment, program);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(Value.Void, result);
        Assert.Equal(expectedOutput, environment.Output);
    }

    public static TheoryData<List<Instruction>, string, string> GetPrintAndVoidResultData()
    {
        return new TheoryData<List<Instruction>, string, string>
        {
            {
                [
                    new Instruction(InstructionCode.Push, "Hello"),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                string.Empty,
                "Hello"
            },
            {
                [
                    new Instruction(InstructionCode.Push, 762),
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.Print),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                string.Empty,
                "762"
            },
        };
    }

    [CulturedTheory(["ru-RU", "en-US"])]
    [MemberData(nameof(GetInputAsResultData))]
    public void Can_read_input_word_as_program_result(
        List<Instruction> program,
        string input,
        Value expected)
    {
        FakeEnvironment environment = new();
        environment.AddInput(input);
        MinionVm vm = new(environment, program);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(expected, result);
        Assert.Empty(environment.Output);
    }

    public static TheoryData<List<Instruction>, string, Value> GetInputAsResultData()
    {
        return new TheoryData<List<Instruction>, string, Value>
        {
            {
                [
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.InputInt),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "42",
                new Value(42)
            },
            {
                [
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.InputFloat),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "3.25",
                new Value(3.25)
            },
            {
                [
                    new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.InputString),
                    new Instruction(InstructionCode.StoreResult),
                    new Instruction(InstructionCode.Push, 0),
                    new Instruction(InstructionCode.Halt),
                ],
                "token-with-dashes",
                new Value("token-with-dashes")
            },
        };
    }

    [Fact]
    public void InputInt_throws_on_invalid_token()
    {
        FakeEnvironment environment = new();
        environment.AddInput("not-an-int");
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.InputInt),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<FormatException>(() =>
        {
            _ = vm.RunProgram();
        });
    }

    [Fact]
    public void InputInt_throws_when_no_input_word()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.InputInt),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<EndOfStreamException>(() =>
        {
            _ = vm.RunProgram();
        });
    }

    [Fact]
    public void InputString_throws_when_no_input_word()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.InputString),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<EndOfStreamException>(() =>
        {
            _ = vm.RunProgram();
        });
    }

    [Fact]
    public void StringLength_returns_code_unit_count()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.Push, "abc"),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.StringLength),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(new Value(3), result);
    }

    [Fact]
    public void StringSubstring_extracts_by_start_and_count()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.Push, "hello"),
            new Instruction(InstructionCode.Push, 1),
            new Instruction(InstructionCode.Push, 3),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.StringSubstring),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);
        Value result = vm.RunProgram();

        Assert.Equal(0, vm.ExitCode);
        Assert.Equal(new Value("ell"), result);
    }

    [Fact]
    public void StringSubstring_throws_when_out_of_range()
    {
        FakeEnvironment environment = new();
        MinionVm vm = new(environment, [
            new Instruction(InstructionCode.Push, "ab"),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Push, 5),
            new Instruction(InstructionCode.CallBuiltin, (int)BuiltinFunctionCode.StringSubstring),
            new Instruction(InstructionCode.StoreResult),
            new Instruction(InstructionCode.Push, 0),
            new Instruction(InstructionCode.Halt),
        ]);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = vm.RunProgram();
        });
    }
}
