using TLPMinion.Ast.Expressions;
using TLPMinion.Runtime;
using TLPMinion.Semantics;
using TLPMinion.VirtualMachine;
using TLPMinion.VirtualMachine.Instructions;
using TLPMinion.VMCodegen;

namespace TLPMinion.Interpreter;

public class MinionInterpreter
{
    private readonly IEnvironment _environment;
    private int _exitCode;

    public MinionInterpreter(IEnvironment environment)
    {
        _environment = environment;
    }

    public int ExitCode => _exitCode;

    public Value Execute(string code)
    {
        Parser.Parser parser = new(code);
        Expression program = parser.ParseProgram();

        SemanticsChecker checker = new();
        checker.Check(program);

        MinionVmCodegen codegen = new();
        IReadOnlyList<Instruction> instructions = codegen.GenerateCode(program);

        MinionVm vm = new(_environment, instructions);
        Value result = vm.RunProgram();
        _exitCode = vm.ExitCode;
        return result;
    }
}
