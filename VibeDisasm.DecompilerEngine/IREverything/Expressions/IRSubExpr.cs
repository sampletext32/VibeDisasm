using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Expressions;

/// <summary>
/// Represents a subtraction expression in IR.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRSubExpr : IRExpression
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }

    public override List<IRExpression> SubExpressions => [Left, Right];

    public IRSubExpr(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IRSubExpr other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitSub(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitSub(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRSubExpr({Left.DebugDisplay} - {Right.DebugDisplay})";
}
