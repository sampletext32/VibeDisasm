using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a bitwise XOR instruction in IR.
/// Example: xor eax, 1 -> IRXorInstruction(eax, 1)
/// </summary>
public sealed class IRXorInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];

    public override IReadOnlyList<IRFlagEffect> SideEffects =>
    [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Parity)
    ];

    public override string ToString() => $"{Left} ^= {Right}";

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: result of XOR is zero (means operands are equal)
            IRFlag.Zero => new IRCompareExpr(
                Left,
                Right,
                expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: MSB of result is set (result is negative)
            IRFlag.Sign => new IRCompareExpr(
                new IRXorExpr(Left, Right),
                IRConstantExpr.Int(0),
                expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Special case: xor reg, reg (clearing a register)
            // This is often used to set the zero flag and clear a register
            _ => null // Other flags not directly mappable
        };
    }

    public IRXorInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
