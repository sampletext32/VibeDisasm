using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a shift right logical instruction in IR.
/// Example: shr eax, 2 -> IRShrInstruction(eax, 2)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRShrInstruction : IRInstruction
{
    public IRExpression Value { get; init; }
    public IRExpression ShiftCount { get; init; }
    public override IRExpression? Result => Value;
    public override IReadOnlyList<IRExpression> Operands => [Value, ShiftCount];

    public IRShrInstruction(IRExpression value, IRExpression shiftCount)
    {
        Value = value;
        ShiftCount = shiftCount;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitShr(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitShr(this);

    internal override string DebugDisplay => $"IRShrInstruction({Value.DebugDisplay} >> {ShiftCount.DebugDisplay})";
}
