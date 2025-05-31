using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an increment (INC) instruction in IR.
/// Example: inc eax -> IRIncInstruction(eax)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRIncInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Target { get; init; }

    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];

    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            IRFlag.Zero => new IRCompareExpr(Target, IRConstantExpr.Int(-1),
                expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            IRFlag.Sign => new IRCompareExpr(Target, IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            _ => null // Other flags not directly mappable
        };
    }

    public IRIncInstruction(IRExpression target) => Target = target;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitInc(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitInc(this);

    internal override string DebugDisplay => $"IRIncInstruction({Target.DebugDisplay}++)";
}
