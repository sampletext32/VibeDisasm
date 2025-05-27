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
    public IRNotInstruction(IRExpression operand)
    {
        Operand = operand;
    }

    public override string ToString() => $"{Operand} = ~{Operand}";
}
