using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Expressions;

/// <summary>
/// Represents a memory dereference in IR (e.g., *address).
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRDerefExpr : IRExpression
{
    public IRExpression Address { get; init; }

    public override List<IRExpression> SubExpressions => [Address];

    public IRDerefExpr(IRExpression address) => Address = address;

    public override bool Equals(object? obj)
    {
        if (obj is IRDerefExpr other)
        {
            return Address.Equals(other.Address);
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitDeref(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitDeref(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRDerefExpr({Address.DebugDisplay})";
}
