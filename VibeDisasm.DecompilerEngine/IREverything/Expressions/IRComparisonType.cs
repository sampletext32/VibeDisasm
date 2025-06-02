namespace VibeDisasm.DecompilerEngine.IREverything.Expressions;

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
    public static IRComparisonType Invert(this IRComparisonType comparisonType) => comparisonType switch
    {
        IRComparisonType.Equal => IRComparisonType.NotEqual,
        IRComparisonType.NotEqual => IRComparisonType.Equal,
        IRComparisonType.LessThan => IRComparisonType.GreaterThanOrEqual,
        IRComparisonType.LessThanOrEqual => IRComparisonType.GreaterThan,
        IRComparisonType.GreaterThan => IRComparisonType.LessThanOrEqual,
        IRComparisonType.GreaterThanOrEqual => IRComparisonType.LessThan,
        _ => throw new ArgumentOutOfRangeException(nameof(comparisonType), comparisonType, null)
    };
}
