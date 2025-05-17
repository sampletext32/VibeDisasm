namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Base class for all expression nodes that produce a value
/// </summary>
public abstract class IRExpression : IRNode
{
    public IRExpression(IRNodeType nodeType) : base(nodeType)
    {
    }
}
