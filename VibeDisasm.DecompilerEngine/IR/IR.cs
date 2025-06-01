using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Convenience class for IR creation.
/// </summary>
public static class IR
{
    public static IRAddExpr Add(IRExpression left, IRExpression right) => new IRAddExpr(left, right);

    public static IRCompareExpr Compare(IRExpression left, IRExpression right, IRComparisonType comparison) => new IRCompareExpr(left, right, comparison);
}
