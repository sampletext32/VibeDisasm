using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a negation (NEG) instruction in IR.
/// Example: neg eax -> IRNegInstruction(eax)
/// </summary>
public sealed class IRNegInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity)
    ];

    public override string ToString() => $"{Target} = -{Target}";
    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    
    public IRNegInstruction(IRExpression target)
    {
        Target = target;
    }
}
