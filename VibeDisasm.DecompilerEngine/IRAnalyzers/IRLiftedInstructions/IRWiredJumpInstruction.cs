using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IR;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

/// <summary>
/// Represents a jump instruction with a reference to the instruction that set its condition flags.
/// </summary>
public class IRWiredJumpInstruction : IRWrappingInstruction<IRJumpInstruction>
{
    public IRInstruction ConditionInstruction { get; set; }

    public IRWiredJumpInstruction(IRJumpInstruction originalJump, IRInstruction conditionInstruction)
        : base(originalJump)
    {
        ConditionInstruction = conditionInstruction;
    }

    public override string ToString() => ToLangString();

    /// <summary>
    /// Translates the condition to a human-readable string using the context of the condition instruction.
    /// </summary>
    [Pure]
    public string ToLangString()
    {
        if (WrappedInstruction.Condition == null)
            return $"jump to {WrappedInstruction.Target}"; // Unconditional jump

        if (ConditionInstruction is IRCmpInstruction cmpInst)
        {
            // Extract operands from the comparison instruction
            var left = cmpInst.Left;
            var right = cmpInst.Right;

            // Determine the comparison operator based on the jump condition flags
            string comparisonOperator = GetComparisonOperator(WrappedInstruction.Condition);

            return $"if ({left} {comparisonOperator} {right}) jump to {WrappedInstruction.Target}";
        }
        
        // Handle DEC instruction which sets the Sign flag (used to check if result is negative)
        if (ConditionInstruction is IRDecInstruction decInst && 
            WrappedInstruction.Condition is IRCompareExpr { 
                Left: IRFlagExpr flagExpr, 
                Right: IRConstantExpr {Value: bool value}
            } cmp && 
            flagExpr.Flag == IRFlag.Sign)
        {
            string comparisonText = value ? "< 0" : ">= 0";
            return $"if ({decInst.Target} {comparisonText}) jump to {WrappedInstruction.Target}";
        }

        // Fallback if condition instruction is not a recognized type
        return $"if ({TranslateCondition(WrappedInstruction.Condition)}) jump to {WrappedInstruction.Target}";
    }

    [Pure]
    private static string GetComparisonOperator(IRExpression condition)
    {
        // Handle different flag-based comparisons
        return condition switch
        {
            // Direct flag comparisons - Zero flag
            IRCompareExpr
                {
                    Left: IRFlagExpr flagExpr,
                    Right: IRConstantExpr {Value: bool value}
                } cmp
                when flagExpr.Flag == IRFlag.Zero && cmp.Comparison == IRComparisonType.Equal =>
                value
                    ? "=="
                    : "!=",
                    
            // Direct flag comparisons - Sign flag (for decrements/increments)
            IRCompareExpr
                {
                    Left: IRFlagExpr flagExpr,
                    Right: IRConstantExpr {Value: bool value}
                } cmp
                when flagExpr.Flag == IRFlag.Sign && cmp.Comparison == IRComparisonType.Equal =>
                value
                    ? "< 0"  // If Sign is true, result is negative
                    : ">= 0", // If Sign is false, result is non-negative

            // Sign == Overflow means Greater than or Equal (signed)
            IRCompareExpr
                {
                    Left: IRFlagExpr leftFlag,
                    Right: IRFlagExpr rightFlag
                } cmp
                when leftFlag.Flag == IRFlag.Sign && rightFlag.Flag == IRFlag.Overflow && cmp.Comparison == IRComparisonType.Equal =>
                ">=",

            // Sign != Overflow means Less than (signed)
            IRCompareExpr
                {
                    Left: IRFlagExpr leftFlag,
                    Right: IRFlagExpr rightFlag
                } cmp
                when leftFlag.Flag == IRFlag.Sign && rightFlag.Flag == IRFlag.Overflow && cmp.Comparison == IRComparisonType.NotEqual =>
                "<",

            // Carry == false means Above or Equal (unsigned)
            IRCompareExpr
                {
                    Left: IRFlagExpr flagExpr,
                    Right: IRConstantExpr {Value: bool value}
                } cmp
                when flagExpr.Flag == IRFlag.Carry && cmp.Comparison == IRComparisonType.Equal =>
                value
                    ? "<"
                    : ">=", // If Carry is true, it's <, otherwise >=

            // Logical combination of flags
            IRLogicalExpr
                {
                    Operand1: IRCompareExpr {Left: IRFlagExpr zeroFlag},
                    Operand2: IRCompareExpr {Left: IRFlagExpr signFlag, Right: IRFlagExpr overflowFlag},
                    Operation: IRLogicalOperation.Or
                } logic
                when zeroFlag.Flag == IRFlag.Zero && signFlag.Flag == IRFlag.Sign && overflowFlag.Flag == IRFlag.Overflow =>
                "<=", // Zero=1 OR Sign!=Overflow means Less than or Equal

            // Default case
            _ => TranslateCondition(condition)
        };
    }

    [Pure]
    private static string TranslateCondition(IRExpression condition)
    {
        // Handle more general expressions or complex conditions
        return condition.ToString();
    }
}