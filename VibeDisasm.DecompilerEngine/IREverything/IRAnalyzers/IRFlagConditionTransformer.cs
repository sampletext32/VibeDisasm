using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;

/// <summary>
/// Transforms flag-based conditions in IR to higher-level comparison expressions.
/// This class is responsible for recognizing common CPU flag patterns and converting them
/// to more readable comparison expressions (e.g., converting flag checks to less than/greater than).
/// </summary>
public static class IRFlagConditionTransformer
{
    /// <summary>
    /// Transforms a flag-based condition into a higher-level comparison expression based on
    /// the instruction that set the flags.
    /// </summary>
    /// <param name="condition">The flag-based condition to transform</param>
    /// <param name="flagSettingInstruction">The instruction that set the flags used in the condition</param>
    /// <returns>A transformed expression, or null if the pattern isn't recognized</returns>
    [Pure]
    public static IRExpression? TransformCondition(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        // Try each transformation pattern in sequence
        return TryTransformDirectFlagComparison(condition, flagSettingInstruction)
            ?? TryTransformSignOverflowNotEqual(condition, flagSettingInstruction)
            ?? TryTransformSignOverflowEqual(condition, flagSettingInstruction)
            ?? TryTransformJNLEPattern(condition, flagSettingInstruction)
            ?? TryTransformJLEPattern(condition, flagSettingInstruction)
            ?? TryTransformLogicalExpression(condition, flagSettingInstruction);
    }

    /// <summary>
    /// Transforms direct flag comparisons via instruction's flag translator
    /// </summary>
    [Pure]
    private static IRExpression? TryTransformDirectFlagComparison(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        if (condition is not IRCompareExpr { Left: IRFlagExpr flagExpr, Right: IRConstantExpr { Value: bool value } })
        {
            return null;
        }

        return new FlagTranslateConditionVisitor(flagExpr.Flag, value).Visit(flagSettingInstruction);
    }

    /// <summary>
    /// Transforms Sign != Overflow comparison after CMP instruction (less than comparison)
    /// </summary>
    [Pure]
    private static IRExpression? TryTransformSignOverflowNotEqual(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        if (condition is not IRCompareExpr
            {
                Left: IRFlagExpr signFlag,
                Right: IRFlagExpr overflowFlag,
                Comparison: IRComparisonType.NotEqual
            }
            || signFlag.Flag != IRFlag.Sign
            || overflowFlag.Flag != IRFlag.Overflow
            || flagSettingInstruction is not IRCmpInstruction cmpInst)
        {
            return null;
        }

        return IR.CompareLessThan(
            cmpInst.Left,
            cmpInst.Right
        );
    }

    /// <summary>
    /// Transforms Sign == Overflow comparison after CMP instruction (greater than or equal comparison)
    /// </summary>
    [Pure]
    private static IRExpression? TryTransformSignOverflowEqual(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        if (condition is not IRCompareExpr
            {
                Left: IRFlagExpr signFlag,
                Right: IRFlagExpr overflowFlag,
                Comparison: IRComparisonType.Equal
            }
            || signFlag.Flag != IRFlag.Sign
            || overflowFlag.Flag != IRFlag.Overflow
            || flagSettingInstruction is not IRCmpInstruction cmpInst)
        {
            return null;
        }

        return IR.CompareGreaterThanOrEqual(
            cmpInst.Left,
            cmpInst.Right
        );
    }

    /// <summary>
    /// Transforms JNLE pattern: (Zero == false AND Sign == Overflow)
    /// </summary>
    [Pure]
    private static IRExpression? TryTransformJNLEPattern(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        if (condition is not IRLogicalExpr
            {
                Operation: IRLogicalOperation.And,
                Left: IRCompareExpr
                {
                    Left: IRFlagExpr { Flag: IRFlag.Zero },
                    Right: IRConstantExpr { Value: false },
                    Comparison: IRComparisonType.Equal
                },
                Right: IRCompareExpr
                {
                    Left: IRFlagExpr { Flag: IRFlag.Sign },
                    Right: IRFlagExpr { Flag: IRFlag.Overflow },
                    Comparison: IRComparisonType.Equal
                }
            }
            || flagSettingInstruction is not IRCmpInstruction cmpInst)
        {
            return null;
        }

        // This is the JNLE pattern - translate directly to Greater Than
        return IR.CompareGreaterThan(
            cmpInst.Left,
            cmpInst.Right
        );
    }

    /// <summary>
    /// Transforms JLE pattern: (Zero == true OR Sign != Overflow)
    /// </summary>
    [Pure]
    private static IRExpression? TryTransformJLEPattern(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        if (!IsLessThanOrEqualPattern(condition) || flagSettingInstruction is not IRCmpInstruction cmpInst)
        {
            return null;
        }

        // This is the JLE pattern - translate directly to Less Than or Equal
        return IR.CompareLessThanOrEqual(
            cmpInst.Left,
            cmpInst.Right
        );
    }

    /// <summary>
    /// Transforms logical combinations of flag conditions, including TEST instructions and recursive transforms
    /// </summary>
    [Pure]
    private static IRExpression? TryTransformLogicalExpression(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        if (condition is not IRLogicalExpr logicalExpr)
        {
            return null;
        }

        // TEST reg, reg with Zero OR (Sign != Overflow) - less than or equal to zero pattern
        if (flagSettingInstruction is IRTestInstruction testInst
            && testInst.Left.Equals(testInst.Right)
            && IsLessThanOrEqualPattern(logicalExpr))
        {
            return IR.CompareLessThanOrEqual(
                testInst.Left,
                IR.Int(0)
            );
        }

        // Recursive transformation of logical expressions
        var transformedOp1 = TransformCondition(logicalExpr.Left, flagSettingInstruction);
        var transformedOp2 = TransformCondition(logicalExpr.Right, flagSettingInstruction);

        if (transformedOp1 != null && transformedOp2 != null)
        {
            return new IRLogicalExpr(transformedOp1, transformedOp2, logicalExpr.Operation);
        }

        return null;
    }

    /// <summary>
    /// Checks if an expression matches the Less Than Or Equal pattern: (Zero == true OR Sign != Overflow)
    /// Used by both JLE and TEST instructions
    /// </summary>
    [Pure]
    private static bool IsLessThanOrEqualPattern(IRExpression expr)
    {
        return expr is IRLogicalExpr
        {
            Operation: IRLogicalOperation.Or,
            Left: IRCompareExpr
            {
                Left: IRFlagExpr { Flag: IRFlag.Zero },
                Right: IRConstantExpr { Value: true },
                Comparison: IRComparisonType.Equal
            },
            Right: IRCompareExpr
            {
                Left: IRFlagExpr { Flag: IRFlag.Sign },
                Right: IRFlagExpr { Flag: IRFlag.Overflow },
                Comparison: IRComparisonType.NotEqual
            }
        };
    }
}
