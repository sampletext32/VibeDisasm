using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a subtract instruction in IR.
/// Example: sub eax, ebx -> IRSubInstruction(eax, ebx)
/// </summary>
public sealed class IRSubInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Destination { get; init; }
    public IRExpression Source { get; init; }
    public override IRExpression? Result => Destination;

    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Destination} -= {Source}";
    public override IReadOnlyList<IRExpression> Operands => [Destination, Source];

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: left == right
            IRFlag.Zero => new IRCompareExpr(
                Destination,
                Source,
                expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            // Sign flag: result < 0, which means left < right for signed comparison
            IRFlag.Sign => new IRCompareExpr(
                Destination,
                Source,
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // Carry flag: unsigned overflow (happens when left < right for unsigned comparison)
            IRFlag.Carry => new IRCompareExpr(
                Destination,
                Source,
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            _ => null // Other flags not directly mappable
        };
    }

    public IRSubInstruction(IRExpression destination, IRExpression source)
    {
        Destination = destination;
        Source = source;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
