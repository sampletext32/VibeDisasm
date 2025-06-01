using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;

public abstract class IRWrappingInstruction<T> : IRInstruction
    where T : IRInstruction
{
    public T WrappedInstruction { get; init; }

    public override IRExpression? Result => WrappedInstruction.Result;
    public override IReadOnlyList<IRExpression> Operands => WrappedInstruction.Operands;

    public IRWrappingInstruction(T wrappedInstruction) => WrappedInstruction = wrappedInstruction;
}
