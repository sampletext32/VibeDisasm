using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Model;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Base class for all IR instructions.
/// Example: mov eax, ebx -> IRMoveInstruction
/// </summary>
public abstract class IRInstruction : IRNode
{
    public abstract IRExpression? Result { get; }
    public abstract IReadOnlyList<IRExpression> Operands { get; }

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
