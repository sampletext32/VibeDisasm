using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a move instruction in IR.
/// Example: mov eax, ebx -> IRMoveInstruction(eax, ebx)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
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

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitMove(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitMove(this);

    internal override string DebugDisplay => $"IRMoveInstruction({Destination.DebugDisplay} = {Source.DebugDisplay})";
}
