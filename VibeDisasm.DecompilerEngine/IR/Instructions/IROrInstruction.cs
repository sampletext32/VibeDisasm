using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a bitwise OR instruction in IR.
/// Example: or eax, 1 -> IROrInstruction(eax, 1)
/// </summary>
public sealed class IROrInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public override string ToString() => $"{Left} |= {Right}";

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: result of OR is zero (both operands are zero)
            IRFlag.Zero => new IRLogicalExpr(
                new IRCompareExpr(Left, IRConstantExpr.Int(0), IRComparisonType.Equal),
                new IRCompareExpr(Right, IRConstantExpr.Int(0), IRComparisonType.Equal),
                expectedValue ? IRLogicalOperation.And : IRLogicalOperation.Or),

            // Sign flag: MSB of result is set (result is negative)
            IRFlag.Sign => new IRCompareExpr(
                new IRLogicalExpr(Left, Right, IRLogicalOperation.Or),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            _ => null // Other flags not directly mappable
        };
    }

    public IROrInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitOr(this);
}
