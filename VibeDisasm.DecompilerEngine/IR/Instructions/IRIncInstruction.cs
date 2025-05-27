using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an increment (INC) instruction in IR.
/// Example: inc eax -> IRIncInstruction(eax)
/// </summary>
public sealed class IRIncInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new IRFlagEffect(IRFlag.Zero),
        new IRFlagEffect(IRFlag.Sign),
        new IRFlagEffect(IRFlag.Overflow),
        new IRFlagEffect(IRFlag.Parity),
        new IRFlagEffect(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Target}++";
    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    
    public IRIncInstruction(IRExpression target)
    {
        Target = target;
    }
}
