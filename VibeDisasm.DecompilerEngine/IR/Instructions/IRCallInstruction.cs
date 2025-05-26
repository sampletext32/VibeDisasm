using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a call instruction in IR.
/// Example: call 0x401000 -> IRCallInstruction(0x401000)
/// </summary>
public sealed class IRCallInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    public IRCallInstruction(IRExpression target)
    {
        Target = target;
    }
}
