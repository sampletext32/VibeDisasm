using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a bitwise NOT instruction in IR.
/// Example: not eax -> IRNotInstruction(eax)
/// </summary>
public sealed class IRNotInstruction : IRInstruction
{
    public IRExpression Operand { get; init; }
    public override IRExpression? Result => Operand;
    public override IReadOnlyList<IRExpression> Operands => [Operand];
    
    // NOT instruction clears OF and CF flags, and affects SF, ZF, and PF
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Overflow),
        new(IRFlag.Carry),
        new(IRFlag.Sign),
        new(IRFlag.Zero),
        new(IRFlag.Parity)
    ];
    
    public IRNotInstruction(IRExpression operand)
    {
        Operand = operand;
    }

    public override string ToString() => $"{Operand} = ~{Operand}";
}
