namespace TLPMinion.VMCodegen;

public sealed class CodegenSymbolsTable
{
    private readonly CodegenSymbolsTable? _parent;
    private readonly int _depth;

    public CodegenSymbolsTable(CodegenSymbolsTable? parent)
    {
        _parent = parent;
        _depth = (parent?.Depth ?? 0) + 1;
    }

    public int Depth => _depth;

    public CodegenSymbolsTable? Parent => _parent;
}
