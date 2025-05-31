using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a stack POP instruction in IR.
/// Example: pop eax -> IRPopInstruction(eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRPopInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];

    public IRPopInstruction(IRExpression target) => Target = target;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitPop(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitPop(this);

    internal override string DebugDisplay => $"IRPopInstruction(pop {Target.DebugDisplay})";
}
