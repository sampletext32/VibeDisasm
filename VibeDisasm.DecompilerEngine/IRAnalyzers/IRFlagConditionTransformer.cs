using VibeDisasm.DecompilerEngine.IR;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers;

public static class IRFlagConditionTransformer
{
    public static IRExpression? TransformCondition(IRExpression condition, IRInstruction flagSettingInstruction)
    {
        // Direct flag comparisons via instruction's flag translator
        if (condition is IRCompareExpr {Left: IRFlagExpr flagExpr, Right: IRConstantExpr {Value: bool value}} flagCmp
            && flagSettingInstruction is IIRFlagTranslatingInstruction flagTranslator)
        {
            var transformedFlags = flagTranslator.GetFlagCondition(flagExpr.Flag, value);
            return transformedFlags;
        }

        {
            // Sign != Overflow comparison after CMP instruction (less than comparison)
            if (condition is IRCompareExpr
                {
                    Left: IRFlagExpr signFlag,
                    Right: IRFlagExpr overflowFlag,
                    Comparison: IRComparisonType.NotEqual
                } signOverflowCmp
                && signFlag.Flag == IRFlag.Sign
                && overflowFlag.Flag == IRFlag.Overflow
                && flagSettingInstruction is IRCmpInstruction cmpInst)
            {
                return new IRCompareExpr(
                    cmpInst.Left,
                    cmpInst.Right,
                    IRComparisonType.LessThan
                );
            }
        }

        {
            // Sign == Overflow comparison after CMP instruction (greater than or equal comparison)
            if (condition is IRCompareExpr
                {
                    Left: IRFlagExpr signFlag,
                    Right: IRFlagExpr overflowFlag,
                    Comparison: IRComparisonType.Equal
                } signOverflowCmp
                && signFlag.Flag == IRFlag.Sign
                && overflowFlag.Flag == IRFlag.Overflow
                && flagSettingInstruction is IRCmpInstruction cmpInst)
            {
                return new IRCompareExpr(
                    cmpInst.Left,
                    cmpInst.Right,
                    IRComparisonType.GreaterThanOrEqual
                );
            }
        }

        {
            // JNLE pattern recognition (Zero == false AND Sign == Overflow)
            if (condition is IRLogicalExpr
                {
                    Operation: IRLogicalOperation.And,
                    Operand1: IRCompareExpr
                    {
                        Left: IRFlagExpr {Flag: IRFlag.Zero},
                        Right: IRConstantExpr {Value: false},
                        Comparison: IRComparisonType.Equal
                    },
                    Operand2: IRCompareExpr
                    {
                        Left: IRFlagExpr {Flag: IRFlag.Sign},
                        Right: IRFlagExpr {Flag: IRFlag.Overflow},
                        Comparison: IRComparisonType.Equal
                    }
                } && flagSettingInstruction is IRCmpInstruction cmpInst)
            {
                // This is the JNLE pattern - translate directly to Greater Than
                return new IRCompareExpr(
                    cmpInst.Left,
                    cmpInst.Right,
                    IRComparisonType.GreaterThan
                );
            }
        }

        {
            // JLE pattern recognition (Zero == true OR Sign != Overflow)
            if (condition is IRLogicalExpr
                {
                    Operation: IRLogicalOperation.Or,
                    Operand1: IRCompareExpr
                    {
                        Left: IRFlagExpr {Flag: IRFlag.Zero},
                        Right: IRConstantExpr {Value: true},
                        Comparison: IRComparisonType.Equal
                    },
                    Operand2: IRCompareExpr
                    {
                        Left: IRFlagExpr {Flag: IRFlag.Sign},
                        Right: IRFlagExpr {Flag: IRFlag.Overflow},
                        Comparison: IRComparisonType.NotEqual
                    }
                } && flagSettingInstruction is IRCmpInstruction cmpInst)
            {
                // This is the JLE pattern - translate directly to Less Than or Equal
                return new IRCompareExpr(
                    cmpInst.Left,
                    cmpInst.Right,
                    IRComparisonType.LessThanOrEqual
                );
            }
        }

        // Logical combinations of flag conditions
        if (condition is IRLogicalExpr logicalExpr)
        {
            // TEST reg, reg with Zero OR (Sign != Overflow) - less than or equal to zero pattern
            if (flagSettingInstruction is IRTestInstruction testInst
                && testInst.Left.Equals(testInst.Right)
                && IsLessThanOrEqualZeroPattern(logicalExpr))
            {
                return new IRCompareExpr(
                    testInst.Left,
                    IRConstantExpr.Int(0),
                    IRComparisonType.LessThanOrEqual
                );
            }

            // Recursive transformation of logical expressions
            var transformedOp1 = TransformCondition(logicalExpr.Operand1, flagSettingInstruction);
            var transformedOp2 = TransformCondition(logicalExpr.Operand2, flagSettingInstruction);

            if (transformedOp1 != null && transformedOp2 != null)
            {
                return new IRLogicalExpr(transformedOp1, transformedOp2, logicalExpr.Operation);
            }
        }

        return null;
    }

    private static bool IsLessThanOrEqualZeroPattern(IRLogicalExpr expr)
    {
        return expr is
        {
            Operation: IRLogicalOperation.Or,
            Operand1: IRCompareExpr
            {
                Left: IRFlagExpr {Flag: IRFlag.Zero},
                Right: IRConstantExpr {Value: true},
                Comparison: IRComparisonType.Equal
            },
            Operand2: IRCompareExpr
            {
                Left: IRFlagExpr {Flag: IRFlag.Sign},
                Right: IRFlagExpr {Flag: IRFlag.Overflow},
                Comparison: IRComparisonType.NotEqual
            }
        };
    }
}