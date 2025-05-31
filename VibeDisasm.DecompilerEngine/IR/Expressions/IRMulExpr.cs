namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a multiplication expression in IR (for address calculations).
/// </summary>
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
    public override string ToString() => $"{Left} * {Right}";
    
    public override bool Equals(object? obj)
    {
        if (obj is IRMulExpr other)
        {
            return Left.Equals(other.Left) && Right.Equals(other.Right);
        }
        return false;
    }
}
