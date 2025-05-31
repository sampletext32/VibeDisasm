using System.Collections.Generic;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents an increment (INC) instruction in IR.
/// Example: inc eax -> IRIncInstruction(eax)
/// </summary>
public sealed class IRIncInstruction : IRInstruction, IIRFlagTranslatingInstruction
{
    public IRExpression Target { get; init; }
    
    public override IReadOnlyList<IRFlagEffect> SideEffects => [
        new(IRFlag.Zero),
        new(IRFlag.Sign),
        new(IRFlag.Overflow),
        new(IRFlag.Parity),
        new(IRFlag.Auxiliary)
    ];

    public override string ToString() => $"{Target}++";
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
    
    public IRIncInstruction(IRExpression target)
    {
        Target = target;
    }
}
