using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an add-with-carry (ADC) instruction in IR.
/// Example: adc eax, 1 -> IRAdcInstruction(eax, 1)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRAdcInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;

    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        // ADC is complex because it involves the carry flag from a previous operation
        // This is a simplified model focusing on common usage patterns
        return flag switch
        {
            // Zero flag: result == 0
            IRFlag.Zero => new IRCompareExpr(
                new IRAddExpr(
                    new IRAddExpr(Left, Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IRConstantExpr.Int(0),
                expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: result < 0
            IRFlag.Sign => new IRCompareExpr(
                new IRAddExpr(
                    new IRAddExpr(Left, Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IRConstantExpr.Int(0),
                expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Carry flag: unsigned overflow
            // This is a simplification - true overflow detection for ADC is complex
            IRFlag.Carry => new IRLogicalExpr(
                new IRCompareExpr(
                    new IRAddExpr(Left, Right),
                    Left,
                    IRComparisonType.LessThan
                ), // Left + Right overflows
                new IRFlagExpr(IRFlag.Carry), // OR previous carry was set
                IRLogicalOperation.Or
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public IRAdcInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitAdc(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitAdc(this);

    internal override string DebugDisplay => $"IRAdcInstruction({Left.DebugDisplay} += {Right.DebugDisplay} + CF)";
}
