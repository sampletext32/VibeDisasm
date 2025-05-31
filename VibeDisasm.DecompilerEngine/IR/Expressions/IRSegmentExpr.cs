using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a segment operand in IR.
/// Example: FS -> IRSegment("FS")
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRSegmentExpr : IRExpression
{
    public IRSegment Segment { get; init; }

    public override List<IRExpression> SubExpressions => [];

    public IRSegmentExpr(IRSegment segment) => Segment = segment;

    public override bool Equals(object? obj)
    {
        if (obj is IRSegmentExpr other)
        {
            return Segment == other.Segment;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitSegment(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitSegment(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRSegmentExpr({Segment:G})";
}
