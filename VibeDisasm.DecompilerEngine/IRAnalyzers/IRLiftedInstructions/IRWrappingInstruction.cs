using VibeDisasm.DecompilerEngine.IR;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

public class IRWrappingInstruction<T> : IRInstruction
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