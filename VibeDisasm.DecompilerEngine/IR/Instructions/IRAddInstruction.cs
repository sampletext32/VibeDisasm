using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an add instruction in IR.
/// Example: add eax, 1 -> IRAddInstruction(eax, 1)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRAddInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Destination { get; init; }
    public IRExpression Source { get; init; }
    public override IRExpression? Result => Destination;
    public override IReadOnlyList<IRExpression> Operands => [Destination, Source];

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: result == 0
            IRFlag.Zero => new IRCompareExpr(
                new IRAddExpr(Destination, Source),
                IRConstantExpr.Int(0),
                expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: result < 0
            IRFlag.Sign => new IRCompareExpr(
                new IRAddExpr(Destination, Source),
                IRConstantExpr.Int(0),
                expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Carry flag: unsigned overflow (result < either operand)
            IRFlag.Carry => new IRCompareExpr(
                new IRAddExpr(Destination, Source),
                Destination,
                expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Overflow flag is too complex to express directly

            _ => null // Other flags not directly mappable
        };
    }

    public IRAddInstruction(IRExpression destination, IRExpression source)
    {
        Destination = destination;
        Source = source;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitAdd(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitAdd(this);

    internal override string DebugDisplay => $"IRAddInstruction({Destination.DebugDisplay} += {Source.DebugDisplay})";
}
