using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a multiplication instruction in IR.
/// Example: mul eax, 2 -> IRMulInstruction(eax, 2)
/// </summary>
/// Represents a multiplication instruction in IR.
/// Example: mul eax, 4 -> IRMulInstruction(eax, 4)
/// </summary>
public sealed class IRMulInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Carry),
        new(IRFlag.Overflow)
    ];
    public override string ToString() => $"{Left} *= {Right}";
    public IRMulInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }
}
