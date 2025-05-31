using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a TEST instruction in IR which is commonly used to check if a register is zero.
/// Example: test eax, eax -> IRTestInstruction(eax, eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRTestInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        // Special case for TEST reg, reg (common zero-check pattern)
        if (Left.Equals(Right))
        {
            // For TEST reg, reg, checking Zero flag is equivalent to checking if the register is zero
            if (flag == IRFlag.Zero)
            {
                return new IRCompareExpr(
                    Left,
                    IRConstantExpr.Int(0),
                    expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual
                );
            }

            // For TEST reg, reg, checking Sign flag is equivalent to checking if the register is negative
            if (flag == IRFlag.Sign)
            {
                return new IRCompareExpr(
                    Left,
                    IRConstantExpr.Int(0),
                    expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual
                );
            }
        }

        // For all other cases, TEST behaves like AND but doesn't modify the operands
        return flag switch
        {
            IRFlag.Zero => new IRCompareExpr(
                new IRLogicalExpr(Left, Right, IRLogicalOperation.And),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            IRFlag.Sign => new IRCompareExpr(
                new IRLogicalExpr(Left, Right, IRLogicalOperation.And),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // CF and OF are cleared by TEST
            IRFlag.Carry => expectedValue ? IRConstantExpr.Bool(false) : IRConstantExpr.Bool(true),
            IRFlag.Overflow => expectedValue ? IRConstantExpr.Bool(false) : IRConstantExpr.Bool(true),

            _ => null // Other flags not directly mappable
        };
    }

    public IRTestInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitTest(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitTest(this);

    internal override string DebugDisplay => $"IRTestInstruction(Test({Left.DebugDisplay}, {Right.DebugDisplay}))";
}
