using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a comparison (CMP/TEST) instruction in IR.
/// Example: cmp eax, 1 -> IRCmpInstruction(eax, 1)
/// </summary>
public sealed class IRCmpInstruction : IRInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Left} == {Right}";
    
    public IRCmpInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }
}
