using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a call instruction in IR.
/// Example: call 0x401000 -> IRCallInstruction(0x401000)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRCallInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Target];

    public IRCallInstruction(IRExpression target) => Target = target;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitCall(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitCall(this);

    internal override string DebugDisplay => $"IRCallInstruction(call {Target.DebugDisplay})";
}
