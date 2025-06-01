using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Expressions;

/// <summary>
/// Represents a constant value in IR.
/// Example: 5 -> IRConstant(5)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRConstantExpr : IRExpression
{
    public object Value { get; init; }
    public IRType Type { get; init; }

    public override List<IRExpression> SubExpressions => [];

    public IRConstantExpr(object value, IRType type)
    {
        Value = value;
        Type = type;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IRConstantExpr other)
        {
            return Value.Equals(other.Value) && Type == other.Type;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitConstant(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitConstant(this);

    internal override string DebugDisplay => $"IRConstantExpr({Type.DebugDisplay} {Value:X8})";

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
