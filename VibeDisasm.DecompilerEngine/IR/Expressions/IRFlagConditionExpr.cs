using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IR.Instructions;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a processor flag-based condition for conditional jumps in IR.
/// </summary>
public sealed class IRFlagConditionExpr : IRExpression
{
    public enum ComparisonType
    {
        Equals,
        NotEquals,
        And,
        Or
    }
    
    public string Condition { get; init; }
    
    // For backward compatibility
    public IRFlagConditionExpr(string condition) => Condition = condition;
    
    // Simple flag comparison (e.g., ZF == 1)
    public IRFlagConditionExpr(IRFlag flag, bool value)
        => Condition = $"{flag} == {(value ? "1" : "0")}";
    
    // Flag comparison with another flag (e.g., SF == OF)
    public IRFlagConditionExpr(IRFlag flag1, ComparisonType comparison, IRFlag flag2)
        => Condition = $"{flag1} {GetComparisonOperator(comparison)} {flag2}";
    
    // Compound condition with two subconditions (e.g., ZF == 0 && SF == OF)
    public IRFlagConditionExpr(IRFlagConditionExpr left, ComparisonType comparison, IRFlagConditionExpr right)
        => Condition = $"{left} {GetLogicalOperator(comparison)} {right}";
    
    [Pure]
    private static string GetComparisonOperator(ComparisonType comparison) => comparison switch
    {
        ComparisonType.Equals => "==",
        ComparisonType.NotEquals => "!=",
        _ => "=="
    };
    
    [Pure]
    private static string GetLogicalOperator(ComparisonType comparison) => comparison switch
    {
        ComparisonType.And => "&&",
        ComparisonType.Or => "||",
        _ => "&&"
    };

    [Pure]
    public override string ToString() => Condition;
}
