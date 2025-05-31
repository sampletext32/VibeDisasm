using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Base class for all IR instructions.
/// Example: mov eax, ebx -> IRMoveInstruction
/// </summary>
public abstract class IRInstruction : IRNode
{
    public abstract IRExpression? Result { get; }
    public abstract IReadOnlyList<IRExpression> Operands { get; }
    public virtual IReadOnlyList<IRFlagEffect> SideEffects => [];

    public IEnumerable<T> EnumerateAllExpressionsOfType<T>()
        where T : IRExpression
    {
        foreach (var operand in Operands)
        {
            if (operand is T t)
            {
                yield return t;
            }

            foreach (var subExpr in operand.SubExpressions.SelectMany(x => x.EnumerateExpressionOfType<T>()))
            {
                yield return subExpr;
            }
        }
    }
}
