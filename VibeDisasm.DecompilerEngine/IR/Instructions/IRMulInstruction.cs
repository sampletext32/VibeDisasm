using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a multiplication instruction in IR.
/// Example: mul eax, 2 -> IRMulInstruction(eax, 2)
/// </summary>
public sealed class IRMulInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Left { get; init; }
    public IRExpression Right { get; init; }
    public override IRExpression? Result => Left;
    public override IReadOnlyList<IRExpression> Operands => [Left, Right];
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Carry),
        new(IRFlag.Overflow)
    ];
    public override string ToString() => $"{Left} *= {Right}";
    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Carry flag and Overflow flag in MUL are both set when the high half of the result is non-zero
            // This effectively means the result is too large to fit in the destination register
            IRFlag.Carry or IRFlag.Overflow => new IRCompareExpr(
                new IRMulExpr(Left, Right),
                IRConstantExpr.Uint(0xFFFFFFFF), // Check if result > 32-bit max
                expectedValue ? IRComparisonType.GreaterThan : IRComparisonType.LessThanOrEqual),
                
            _ => null // Other flags not directly mappable
        };
    }
    
    public IRMulInstruction(IRExpression left, IRExpression right)
    {
        Left = left;
        Right = right;
    }


    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
