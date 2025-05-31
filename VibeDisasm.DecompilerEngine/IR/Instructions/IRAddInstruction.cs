using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an add instruction in IR.
/// Example: add eax, 1 -> IRAddInstruction(eax, 1)
/// </summary>
public sealed class IRAddInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Destination { get; init; }
    public IRExpression Source { get; init; }
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Destination, Source];

    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Destination} += {Source}";

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: result == 0
            IRFlag.Zero => new IRCompareExpr(
                new IRAddExpr(Destination, Source),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            // Sign flag: result < 0
            IRFlag.Sign => new IRCompareExpr(
                new IRAddExpr(Destination, Source),
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // Carry flag: unsigned overflow (result < either operand) 
            IRFlag.Carry => new IRCompareExpr(
                new IRAddExpr(Destination, Source),
                Destination,
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // Overflow flag is too complex to express directly

            _ => null // Other flags not directly mappable
        };
    }

    public IRAddInstruction(IRExpression destination, IRExpression source)
    {
        Destination = destination;
        Source = source;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
