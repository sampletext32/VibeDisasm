using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Base class for all IR instructions.
/// Example: mov eax, ebx -> IRMoveInstruction
/// </summary>
public abstract class IRInstruction
{
    public abstract IRExpression? Result { get; }
    public abstract IReadOnlyList<IRExpression> Operands { get; }
    public virtual IReadOnlyList<IRFlagEffect> SideEffects => [];
}
