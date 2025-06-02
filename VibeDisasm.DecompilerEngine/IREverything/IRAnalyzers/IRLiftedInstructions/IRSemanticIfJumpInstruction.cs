using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Abstractions;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;

[DebuggerDisplay("{DebugDisplay}")]
public class IRSemanticIfJumpInstruction : IRWrappingInstruction<IRWiredJumpInstruction>, IIRConditionalJump
{
    public IRExpression Condition { get; }
    public IRExpression Target => WrappedInstruction.Target;

    public IRSemanticIfJumpInstruction(
        IRWiredJumpInstruction wrappedInstruction,
        IRExpression condition
    ) : base(wrappedInstruction) =>
        Condition = condition;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitSemanticIfJump(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitSemanticIfJump(this);
    internal override string DebugDisplay => $"IRUnflaggedJumpInstruction(if ({Condition.DebugDisplay}) goto {WrappedInstruction.Target.DebugDisplay})";
}
