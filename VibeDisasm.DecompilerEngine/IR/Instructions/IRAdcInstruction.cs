using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an add-with-carry (ADC) instruction in IR.
/// Example: adc eax, 1 -> IRAdcInstruction(eax, 1)
/// </summary>
public sealed class IRAdcInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Left} += {Right} + CF";
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];
    public IRAdcInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }
}
