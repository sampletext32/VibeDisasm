using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a return instruction in IR.
/// Example: return eax -> IRReturnInstruction(eax)
/// </summary>
public sealed class IRReturnInstruction : IRInstruction
{
    public IRExpression? Value { get; init; }

    public override IRExpression? Result => null;

    public override IReadOnlyList<IRExpression> Operands => Value is not null
        ? [Value]
        : [];

    public IRReturnInstruction()
    {
    }

    public IRReturnInstruction(IRExpression? value) => Value = value;

    [Pure]
    public override string ToString() => Value is null
        ? "return"
        : $"return {Value}";

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitReturn(this);
}
