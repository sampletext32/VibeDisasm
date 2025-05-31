using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a negation (NEG) instruction in IR.
/// Example: neg eax -> IRNegInstruction(eax)
/// </summary>
public sealed class IRNegInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Target { get; init; }
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Carry),
        new(IRFlag.Overflow),
        new(IRFlag.Parity)
    ];

    public override string ToString() => $"{Target} = -{Target}";
    public override IRExpression? Result => Target;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    
    public IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue)
    {
        return flag switch
        {
            // Zero flag: operand == 0
            IRFlag.Zero => new IRCompareExpr(
                Target,
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),
            
            // Sign flag: result is negative (always true for positive values except 0, always false for negative values)
            IRFlag.Sign => new IRCompareExpr(
                Target,
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.GreaterThan : IRComparisonType.LessThanOrEqual),
            
            // Carry flag: operand != 0 (CF is set for all non-zero values)
            IRFlag.Carry => new IRCompareExpr(
                Target,
                IRConstantExpr.Int(0),
                expectedValue ? IRComparisonType.NotEqual : IRComparisonType.Equal),
                
            _ => null // Other flags not directly mappable
        };
    }
    
    public IRNegInstruction(IRExpression target)
    {
        Target = target;
    }


    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
