using System.Diagnostics.Contracts;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a NOT in IR (e.g., ~value).
/// </summary>
public sealed class IRNotExpr : IRExpression
{
    public IRExpression Value { get; init; }

    public override List<IRExpression> SubExpressions => [Value];

    public IRNotExpr(IRExpression value) => Value = value;

    [Pure]
    public override string ToString() => $"~{Value}";

    public override bool Equals(object? obj)
    {
        if (obj is IRNotExpr other)
        {
            return Value.Equals(other.Value);
        }

        return false;
    }
}