using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Base class for all IR expressions.
/// Example: eax, 42, eax + 1
/// </summary>
public abstract class IRExpression : IRNode
{
    public abstract List<IRExpression> SubExpressions { get; }
    
    public IEnumerable<T> EnumerateExpressionOfType<T>()
        where T : IRExpression
    {
        if (this is T t)
            yield return t;

        foreach (var subExpr in SubExpressions)
        {
            foreach (var subResult in subExpr.EnumerateExpressionOfType<T>())
            {
                yield return subResult;
            }
        }
    }
}
