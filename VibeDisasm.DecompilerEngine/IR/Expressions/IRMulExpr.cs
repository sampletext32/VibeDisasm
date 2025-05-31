using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a multiplication expression in IR (for address calculations).
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRMulExpr : IRExpression
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override List<IRExpression> SubExpressions => [Left, Right];
    public IRMulExpr(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IRMulExpr other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitMul(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitMul(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRMulExpr({Left.DebugDisplay} * {Right.DebugDisplay})";
}
