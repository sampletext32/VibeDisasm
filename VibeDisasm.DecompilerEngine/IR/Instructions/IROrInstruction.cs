using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a bitwise OR instruction in IR.
/// Example: or eax, 1 -> IROrInstruction(eax, 1)
/// </summary>
public sealed class IROrInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new IRFlagEffect(IRFlag.Zero),
        new IRFlagEffect(IRFlag.Sign),
        new IRFlagEffect(IRFlag.Parity)
    ];

    public override string ToString() => $"{Left} |= {Right}";
    
    public IROrInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }
}
