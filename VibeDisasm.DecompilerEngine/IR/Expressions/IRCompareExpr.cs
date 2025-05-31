using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

[DebuggerDisplay("{DebugDisplay}")]
public class IRCompareExpr : IRExpression
{
    public IRExpression Left { get; set; }
    public IRExpression Right { get; set; }
    public IRComparisonType Comparison { get; set; }

    public override List<IRExpression> SubExpressions => [Left, Right];

    public IRCompareExpr(IRExpression left, IRExpression right, IRComparisonType comparison)
    {
        Left = left;
        Right = right;
        Comparison = comparison;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IRCompareExpr other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right) && Comparison == other.Comparison;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitCompare(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitCompare(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRCompareExpr({Left.DebugDisplay} {Comparison} {Right.DebugDisplay})";
}
