using VibeDisasm.DecompilerEngine.IR.Expressions;

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
}