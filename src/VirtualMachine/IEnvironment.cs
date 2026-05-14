namespace TLPMinion.VirtualMachine;

public interface IEnvironment
{
    string ReadWord();

    void Print(string text);
}
