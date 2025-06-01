using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an add-with-carry (ADC) instruction in IR.
/// Example: adc eax, 1 -> IRAdcInstruction(eax, 1)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRAdcInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;

    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRAdcInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitAdc(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitAdc(this);

    internal override string DebugDisplay => $"IRAdcInstruction({Left.DebugDisplay} += {Right.DebugDisplay} + CF)";
}
