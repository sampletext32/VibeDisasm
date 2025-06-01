using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a subtract instruction in IR.
/// Example: sub eax, ebx -> IRSubInstruction(eax, ebx)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
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

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitSub(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitSub(this);

    internal override string DebugDisplay => $"IRSubInstruction({Destination.DebugDisplay} -= {Source.DebugDisplay})";
}
