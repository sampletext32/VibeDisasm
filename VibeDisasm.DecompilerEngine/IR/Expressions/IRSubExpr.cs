using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a subtraction expression in IR.
/// </summary>
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

    public override string ToString() => $"{Left} - {Right}";

    public override bool Equals(object? obj)
    {
        if (obj is IRSubExpr other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
