using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

public class IRUnflaggedJumpInstruction : IRWrappingInstruction<IRWiredJumpInstruction>
{
    public IRExpression Condition { get; }

    public IRUnflaggedJumpInstruction(
        IRWiredJumpInstruction wrappedInstruction,
        IRExpression condition) : base(wrappedInstruction)
    {
        Condition = condition;
    }

    public override string ToString() => $"if ({Condition}) goto {WrappedInstruction.Target}";

    public override void Accept(IIRNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
