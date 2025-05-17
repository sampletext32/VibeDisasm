namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Base class for all statement nodes that represent actions or control flow
/// </summary>
public abstract class IRStatement : IRNode
{
    public IRStatement(IRNodeType nodeType) : base(nodeType)
    {
    }
}
