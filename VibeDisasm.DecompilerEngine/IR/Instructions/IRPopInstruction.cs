using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a stack POP instruction in IR.
/// Example: pop eax -> IRPopInstruction(eax)
/// </summary>
public sealed class IRPopInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    public IRPopInstruction(IRExpression target)
    {
        Target = target;
    }

    public override string ToString() => $"pop {Target}";
}
