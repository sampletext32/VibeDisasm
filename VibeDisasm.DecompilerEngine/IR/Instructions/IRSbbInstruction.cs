using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a subtract-with-borrow (SBB) instruction in IR.
/// Example: sbb eax, 1 -> IRSbbInstruction(eax, 1)
/// </summary>
public sealed class IRSbbInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new IRFlagEffect(IRFlag.Zero),
        new IRFlagEffect(IRFlag.Sign),
        new IRFlagEffect(IRFlag.Carry),
        new IRFlagEffect(IRFlag.Overflow),
        new IRFlagEffect(IRFlag.Parity),
        new IRFlagEffect(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Left} -= {Right} - CF";
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];
    public IRSbbInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }
}
