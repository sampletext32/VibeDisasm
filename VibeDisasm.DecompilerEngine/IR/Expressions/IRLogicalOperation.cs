namespace VibeDisasm.DecompilerEngine.IR.Expressions;

public enum IRLogicalOperation
{
    And,
    Or
}

public static class IRLogicalOperationExtensions
{
    public static string ToLangString(this IRLogicalOperation operation) => operation switch
    {
        IRLogicalOperation.And => "&&",
        IRLogicalOperation.Or => "||",
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
    };
}
