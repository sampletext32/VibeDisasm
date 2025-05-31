using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a bitwise AND instruction in IR.
/// Example: and eax, 1 -> IRAndInstruction(eax, 1)
/// </summary>
public sealed class IRAndInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public override string ToString() => $"{Left} &= {Right}";

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: result of AND is zero (common test pattern)
            IRFlag.Zero => new IRCompareExpr(
                new IRLogicalExpr(Left, Right, IRLogicalOperation.And),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            // Sign flag: MSB of result is set (result is negative)
            IRFlag.Sign => new IRCompareExpr(
                new IRLogicalExpr(Left, Right, IRLogicalOperation.And),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            _ => null // Other flags not directly mappable
        };
    }

    public IRAndInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitAnd(this);
}
