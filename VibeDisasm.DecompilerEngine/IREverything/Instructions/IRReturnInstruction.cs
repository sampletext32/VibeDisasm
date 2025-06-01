using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a return instruction in IR.
/// Example: return eax -> IRReturnInstruction(eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
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

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitReturn(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitReturn(this);

    internal override string DebugDisplay => Value is null
        ? "IRReturnInstruction(return)"
        : $"IRReturnInstruction(return {Value.DebugDisplay})";
}
