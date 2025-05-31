using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a bitwise NOT instruction in IR.
/// Example: not eax -> IRNotInstruction(eax)
/// </summary>
public sealed class IRNotInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Operand { get; init; }
    public override IRExpression? Result => Operand;
    public override IReadOnlyList<IRExpression> Operands => [Operand];

    public IRNotInstruction(IRExpression operand)
    {
        Operand = operand;
    }

    public override string ToString() => $"{Operand} = ~{Operand}";

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: ~operand == 0 (which means operand == -1)
            IRFlag.Zero => new IRCompareExpr(
                Operand,
                IRConstantExpr.Int(-1),
                expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            // Sign flag: result is negative (highest bit is set)
            IRFlag.Sign => new IRCompareExpr(
                new IRNotExpr(Operand),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // Carry flag: always cleared to 0 by NOT instruction
            IRFlag.Carry => expectedValue ?
                IRConstantExpr.Bool(false) : // If expected true, never happens
                IRConstantExpr.Bool(true),   // If expected false, always happens

            // Overflow flag: always cleared to 0 by NOT instruction
            IRFlag.Overflow => expectedValue ?
                IRConstantExpr.Bool(false) : // If expected true, never happens
                IRConstantExpr.Bool(true),   // If expected false, always happens

            _ => null // Other flags not directly mappable
        };
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitNot(this);
}
