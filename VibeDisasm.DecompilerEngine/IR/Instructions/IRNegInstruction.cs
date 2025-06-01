using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a negation (NEG) instruction in IR.
/// Example: neg eax -> IRNegInstruction(eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRNegInstruction : IRInstruction
{
    public IRExpression Target { get; init; }

    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];

    public IRNegInstruction(IRExpression target) => Target = target;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitNeg(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitNeg(this);

    internal override string DebugDisplay => $"IRNegInstruction({Target.DebugDisplay} = -{Target.DebugDisplay})";
}
