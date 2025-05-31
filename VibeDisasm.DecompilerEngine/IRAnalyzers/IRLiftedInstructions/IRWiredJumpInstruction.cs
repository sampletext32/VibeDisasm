using System.Diagnostics;
using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IR;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

/// <summary>
/// Represents a jump instruction with a reference to the instruction that set its condition flags.
/// </summary>
public class IRWiredJumpInstruction : IRWrappingInstruction<IRJumpInstruction>
{
    public IRInstruction ConditionInstruction { get; set; }

    public IRExpression? Condition => WrappedInstruction.Condition;
    public IRExpression Target => WrappedInstruction.Target;

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

        return WrappedInstruction.ToString();

        // Handle direct flag comparisons
        if (WrappedInstruction.Condition is IRCompareExpr { Left: IRFlagExpr flagExpr, Right: IRConstantExpr { Value: bool value } }
            && ConditionInstruction is IIRFlagTranslatingInstruction flagTranslatingInstruction)
        {
            // Ask the condition instruction to create a high-level expression for this flag test
            var highLevelCondition = flagTranslatingInstruction.GetFlagCondition(flagExpr.Flag, value);
        
            if (highLevelCondition != null)
            {
                return $"if ({highLevelCondition}) jump to {WrappedInstruction.Target}";
            }
        }
        
        // Handle logical combinations of flag conditions
        if (WrappedInstruction.Condition is IRLogicalExpr logicalExpr)
        {
            // Try to translate the logical expression using our special TEST instruction
            // For TEST reg, reg with Zero OR (Sign != Overflow), this is commonly a "less than or equal" check
            if (logicalExpr.Operation == IRLogicalOperation.Or &&
                logicalExpr.Operand1 is IRCompareExpr {Left: IRFlagExpr zeroFlag, Right: IRConstantExpr {Value: bool zeroValue}} zeroCmp &&
                logicalExpr.Operand2 is IRCompareExpr {Left: IRFlagExpr signFlag, Right: IRFlagExpr overflowFlag} signOverflowCmp &&
                zeroFlag.Flag == IRFlag.Zero &&
                signFlag.Flag == IRFlag.Sign &&
                overflowFlag.Flag == IRFlag.Overflow &&
                zeroCmp.Comparison == IRComparisonType.Equal &&
                signOverflowCmp.Comparison == IRComparisonType.NotEqual &&
                ConditionInstruction is IRTestInstruction testInst &&
                testInst.Left.Equals(testInst.Right))
            {
                // This is the "less than or equal" pattern with TEST reg, reg
                return $"if ({testInst.Left} <= 0) jump to {WrappedInstruction.Target}";
            }

            // Attempt to translate other logical combinations of flags
        }

        if (ConditionInstruction is IRCmpInstruction cmpInst)
        {
            // Extract operands from the comparison instruction
            var left = cmpInst.Left;
            var right = cmpInst.Right;

            // Determine the comparison operator based on the jump condition flags
            string comparisonOperator = GetComparisonOperator(WrappedInstruction.Condition);

            return $"if ({left} {comparisonOperator} {right}) jump to {WrappedInstruction.Target}";
        }

        // Fallback if condition instruction is not a recognized type
        Debugger.Break();
        return $"if (UNTRANSLATED: {WrappedInstruction.Condition}) jump to {WrappedInstruction.Target}";
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
            _ => throw new InvalidOperationException("untranslated condition:" + condition)
        };
    }
}