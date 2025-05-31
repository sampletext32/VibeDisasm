using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Model;

/// <summary>
/// Represents a processor flag modified by an IR instruction.
/// </summary>
public sealed class IRFlagEffect : IRNode
{
    public IRFlag Flag { get; init; }

    public IRFlagEffect(IRFlag flag)
    {
        Flag = flag;
    }

    public override void Accept(IIRNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
