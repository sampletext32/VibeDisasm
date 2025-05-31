using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a MOVZX (Move with Zero-Extend) instruction in IR.
/// Copies the contents of the source operand to the destination operand and zero-extends the value to the size of the destination operand.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRMovzxInstruction : IRInstruction
{
    public IRExpression Source { get; init; }
    public IRExpression Destination { get; init; }
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Source, Destination];

    public IRMovzxInstruction(IRExpression source, IRExpression destination)
    {
        Source = source;
        Destination = destination;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitMovzx(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitMovzx(this);

    internal override string DebugDisplay => $"IRMovzxInstruction({Destination.DebugDisplay} = zeroext {Source.DebugDisplay})";
}
