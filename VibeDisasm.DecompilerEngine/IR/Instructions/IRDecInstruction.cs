using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a decrement (DEC) instruction in IR.
/// Example: dec eax -> IRDecInstruction(eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRDecInstruction : IRInstruction
{
    public IRExpression Target { get; init; }

    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];

    public IRDecInstruction(IRExpression target) => Target = target;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitDec(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitDec(this);

    internal override string DebugDisplay => $"IRDecInstruction({Target.DebugDisplay}--)";
}
