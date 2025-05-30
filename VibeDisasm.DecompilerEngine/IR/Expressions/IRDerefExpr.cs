using System.Diagnostics.Contracts;

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
}