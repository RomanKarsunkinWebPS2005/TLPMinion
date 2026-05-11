namespace TLPMinion.VirtualMachine.Instructions;

public enum InstructionCode
{
    Push,
    Pop,
    DefineVar,
    StoreVar,
    LoadVar,
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    Power,
    Negate,
    CallBuiltin,
    StoreResult,
    Halt,
    PushVars,
    PopVars,
}
