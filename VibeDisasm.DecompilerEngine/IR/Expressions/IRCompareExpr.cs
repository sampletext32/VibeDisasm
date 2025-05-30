namespace VibeDisasm.DecompilerEngine.IR.Expressions;

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

    public override string ToString() => $"{Left} {Comparison.ToLangString()} {Right}";
}

public enum IRComparisonType
{
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual
}

public static class IRComparisonTypeExtensions
{
    public static string ToLangString(this IRComparisonType comparisonType) => comparisonType switch
    {
        IRComparisonType.Equal => "==",
        IRComparisonType.NotEqual => "!=",
        IRComparisonType.LessThan => "<",
        IRComparisonType.LessThanOrEqual => "<=",
        IRComparisonType.GreaterThan => ">",
        IRComparisonType.GreaterThanOrEqual => ">=",
        _ => throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null)
    };
}