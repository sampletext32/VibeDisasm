using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IR.Visitors;

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


    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}