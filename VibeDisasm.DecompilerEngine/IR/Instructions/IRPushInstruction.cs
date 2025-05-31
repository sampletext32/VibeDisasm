using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a stack PUSH instruction in IR.
/// Example: push eax -> IRPushInstruction(eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRPushInstruction : IRInstruction
{
    public IRExpression Value { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Value];

    public IRPushInstruction(IRExpression value) => Value = value;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitPush(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitPush(this);

    internal override string DebugDisplay => $"IRPushInstruction(push({Value.DebugDisplay}))";
}
