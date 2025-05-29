using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an add instruction in IR.
/// Example: add eax, 1 -> IRAddInstruction(eax, 1)
/// </summary>
public sealed class IRAddInstruction : IRInstruction
{
    public IRExpression Destination { get; init; }
    public IRExpression Source { get; init; }
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Destination, Source];
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Destination} += {Source}";
    
    public IRAddInstruction(IRExpression destination, IRExpression source)
    {
        Destination = destination;
        Source = source;
    }
}
