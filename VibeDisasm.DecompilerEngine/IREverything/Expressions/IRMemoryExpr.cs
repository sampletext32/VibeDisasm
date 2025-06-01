using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Expressions;

/// <summary>
/// Represents a memory operand in IR.
/// Example: [ebp+8] -> IRMemory("ebp+8")
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRMemoryExpr : IRExpression
{
    public string Address { get; init; }

    public override List<IRExpression> SubExpressions => [];

    public IRMemoryExpr(string address) => Address = address;

    public override bool Equals(object? obj)
    {
        if (obj is IRMemoryExpr other)
        {
            return Address.Equals(other.Address);
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitMemory(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitMemory(this);

    public override int GetHashCode() => throw new NotImplementedException();

    internal override string DebugDisplay => $"IRMemoryExpr({Address})";
}
