using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a subtract instruction in IR.
/// Example: sub eax, ebx -> IRSubInstruction(eax, ebx)
/// </summary>
public sealed class IRSubInstruction : IRInstruction
{
    public IRExpression Destination { get; init; }
    public IRExpression Source { get; init; }
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Destination, Source];
    public IRSubInstruction(IRExpression destination, IRExpression source)
    {
        Destination = destination;
        Source = source;
    }
}
