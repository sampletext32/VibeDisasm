using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Model;

/// <summary>
/// Represents a processor flag modified by an IR instruction.
/// </summary>
public sealed class IRFlagEffect : IRNode
{
    public IRFlag Flag { get; init; }

    public IRFlagEffect(IRFlag flag) => Flag = flag;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitFlagEffect(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitFlagEffect(this);
    internal override string DebugDisplay => $"IRFlagEffect({Flag:G})";
}
