using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

[DebuggerDisplay("{DebugDisplay}")]
public class IRUnflaggedJumpInstruction : IRWrappingInstruction<IRWiredJumpInstruction>
{
    public IRExpression Condition { get; }
    public IRExpression Target => WrappedInstruction.Target;

    public IRUnflaggedJumpInstruction(
        IRWiredJumpInstruction wrappedInstruction,
        IRExpression condition) : base(wrappedInstruction) =>
        Condition = condition;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitUnflaggedJump(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitUnflaggedJump(this);
    internal override string DebugDisplay => $"IRUnflaggedJumpInstruction(if ({Condition.DebugDisplay}) goto {WrappedInstruction.Target.DebugDisplay})";
}
