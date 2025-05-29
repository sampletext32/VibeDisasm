using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a decrement (DEC) instruction in IR.
/// Example: dec eax -> IRDecInstruction(eax)
/// </summary>
public sealed class IRDecInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Target}--";
    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    
    public IRDecInstruction(IRExpression target)
    {
        Target = target;
    }
}
