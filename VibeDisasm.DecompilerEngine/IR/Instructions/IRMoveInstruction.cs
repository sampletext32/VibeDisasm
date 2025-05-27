using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a move instruction in IR.
/// Example: mov eax, ebx -> IRMoveInstruction(eax, ebx)
/// </summary>
public sealed class IRMoveInstruction : IRInstruction
{
    public IRExpression Destination { get; init; }
    public IRExpression Source { get; init; }
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Destination, Source];
    public IRMoveInstruction(IRExpression destination, IRExpression source)
    {
        Destination = destination;
        Source = source;
    }

    public override string ToString() => $"{Destination} = {Source}";
}
