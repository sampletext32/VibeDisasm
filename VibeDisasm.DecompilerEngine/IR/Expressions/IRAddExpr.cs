namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents an addition expression in IR (for address calculations).
/// </summary>
public sealed class IRAddExpr : IRExpression
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }

    public override List<IRExpression> SubExpressions => [Left, Right];

    public IRAddExpr(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override string ToString() => $"{Left} + {Right}";
}