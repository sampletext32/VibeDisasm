using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Expressions;

/// <summary>
/// Represents a NOT in IR (e.g., ~value).
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRNotExpr : IRExpression
{
    public IRExpression Value { get; init; }

    public override List<IRExpression> SubExpressions => [Value];

    public IRNotExpr(IRExpression value) => Value = value;

    public override bool Equals(object? obj)
    {
        if (obj is IRNotExpr other)
        {
            return Value.Equals(other.Value);
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitNot(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitNot(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRNotExpr({Value.DebugDisplay})";
}
