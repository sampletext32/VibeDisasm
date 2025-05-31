using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a memory dereference in IR (e.g., *address).
/// </summary>
public sealed class IRDerefExpr : IRExpression
{
    public IRExpression Address { get; init; }

    public override List<IRExpression> SubExpressions => [Address];

    public IRDerefExpr(IRExpression address) => Address = address;

    [Pure]
    public override string ToString() => $"*({Address})";

    public override bool Equals(object? obj)
    {
        if (obj is IRDerefExpr other)
        {
            return Address.Equals(other.Address);
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitDeref(this);

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
