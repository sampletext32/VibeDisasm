using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an add instruction in IR.
/// Example: add eax, 1 -> IRAddInstruction(eax, 1)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRAddInstruction : IRInstruction
{
    public IRExpression Destination { get; init; }
    public IRExpression Source { get; init; }
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Destination, Source];

    public IRAddInstruction(IRExpression destination, IRExpression source)
    {
        Destination = destination;
        Source = source;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitAdd(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitAdd(this);

    internal override string DebugDisplay => $"IRAddInstruction({Destination.DebugDisplay} += {Source.DebugDisplay})";
}
