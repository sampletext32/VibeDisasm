namespace VibeDisasm.DecompilerEngine.IR.Expressions;

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
