namespace TLPMinion.Runtime;

public sealed class VoidValue
{
    public static readonly VoidValue Value = new();

    private VoidValue()
    {
    }
}
