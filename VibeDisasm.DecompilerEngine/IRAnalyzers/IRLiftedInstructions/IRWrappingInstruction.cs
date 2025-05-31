using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

public abstract class IRWrappingInstruction<T> : IRInstruction
    where T : IRInstruction
{
    protected readonly T WrappedInstruction;

    public override IRExpression? Result => WrappedInstruction.Result;
    public override IReadOnlyList<IRExpression> Operands => WrappedInstruction.Operands;
    public override IReadOnlyList<IRFlagEffect> SideEffects => WrappedInstruction.SideEffects;

    public IRWrappingInstruction(T wrappedInstruction)
    {
        WrappedInstruction = wrappedInstruction;
    }
}
