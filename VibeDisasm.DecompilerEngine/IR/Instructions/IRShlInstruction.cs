using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a shift left instruction in IR.
/// Example: shl eax, 2 -> IRShlInstruction(eax, 2)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRShlInstruction : IRInstruction
{
    public IRExpression Value { get; init; }
    public IRExpression ShiftCount { get; init; }
    public override IRExpression? Result => Value;
    public override IReadOnlyList<IRExpression> Operands => [Value, ShiftCount];

    public IRShlInstruction(IRExpression value, IRExpression shiftCount)
    {
        Value = value;
        ShiftCount = shiftCount;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitShl(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitShl(this);

    internal override string DebugDisplay => $"IRShlInstruction({Value.DebugDisplay} << {ShiftCount.DebugDisplay})";
}
