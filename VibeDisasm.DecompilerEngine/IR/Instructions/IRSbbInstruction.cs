using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a subtract-with-borrow (SBB) instruction in IR.
/// Example: sbb eax, 1 -> IRSbbInstruction(eax, 1)
/// </summary>
public sealed class IRSbbInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;

    public override IReadOnlyList<IRFlagEffect> SideEffects =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Left} -= {Right} - CF";
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        // SBB is complex because it involves the carry flag from a previous operation
        // This is a simplified model focusing on common usage patterns
        return flag switch
        {
            // Zero flag: result == 0
            IRFlag.Zero => new IRCompareExpr(
                new IRSubExpr(
                    new IRSubExpr(Left, Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IRConstantExpr.Int(0),
                expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: result < 0
            IRFlag.Sign => new IRCompareExpr(
                new IRSubExpr(
                    new IRSubExpr(Left, Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IRConstantExpr.Int(0),
                expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Carry flag: (Left < Right) OR (Left == Right AND carry == 1)
            IRFlag.Carry => new IRLogicalExpr(
                new IRCompareExpr(Left, Right, IRComparisonType.LessThan),
                new IRLogicalExpr(
                    new IRCompareExpr(Left, Right, IRComparisonType.Equal),
                    new IRFlagExpr(IRFlag.Carry),
                    IRLogicalOperation.And
                ),
                IRLogicalOperation.Or
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public IRSbbInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}