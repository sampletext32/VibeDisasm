using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;

namespace VibeDisasm.DecompilerEngine.IREverything.Visitors;

/// <summary>
/// Visitor that translates CPU flag conditions into semantic IR expressions.
/// </summary>
/// <remarks>
/// This visitor is used to convert low-level flag-based conditions (e.g., "jump if zero flag is set")
/// into higher-level expressions (e.g., "if (x == y)").
/// </remarks>
public class FlagTranslateConditionVisitor : BaseIRNodeReturningVisitor<IRExpression?>
{
    private readonly IRFlag _flag;
    private readonly bool _expectedValue;

    public FlagTranslateConditionVisitor(IRFlag flag, bool expectedValue) : base(_ => null)
    {
        _flag = flag;
        _expectedValue = expectedValue;
    }

    public override IRExpression? VisitAdc(IRAdcInstruction instr)
    {
        // ADC is complex because it involves the carry flag from a previous operation
        // This is a simplified model focusing on common usage patterns
        return _flag switch
        {
            // Zero flag: result == 0
            IRFlag.Zero => IR.Compare(
                IR.Add(
                    IR.Add(instr.Left, instr.Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: result < 0
            IRFlag.Sign => IR.Compare(
                IR.Add(
                    IR.Add(instr.Left, instr.Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Carry flag: unsigned overflow
            // This is a simplification - true overflow detection for ADC is complex
            IRFlag.Carry => IR.LogicalOr(
                IR.CompareLessThan(
                    IR.Add(instr.Left, instr.Right),
                    instr.Left
                ), // Left + Right overflows
                new IRFlagExpr(IRFlag.Carry) // OR previous carry was set
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitAdd(IRAddInstruction instr)
    {
        return _flag switch
        {
            // Zero flag: result == 0
            IRFlag.Zero => IR.Compare(
                IR.Add(instr.Destination, instr.Source),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: result < 0
            IRFlag.Sign => IR.Compare(
                IR.Add(instr.Destination, instr.Source),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Carry flag: unsigned overflow (result < either operand)
            IRFlag.Carry => IR.Compare(
                IR.Add(instr.Destination, instr.Source),
                instr.Destination,
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Overflow flag is too complex to express directly

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitAnd(IRAndInstruction instr)
    {
        return _flag switch
        {
            // Zero flag: result of AND is zero (common test pattern)
            IRFlag.Zero => IR.Compare(
                IR.LogicalAnd(instr.Left, instr.Right),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: MSB of result is set (result is negative)
            IRFlag.Sign => IR.Compare(
                IR.LogicalAnd(instr.Left, instr.Right),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitCmp(IRCmpInstruction instr)
    {
        return _flag switch
        {
            IRFlag.Zero => IR.Compare(
                instr.Left,
                instr.Right,
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            IRFlag.Sign => IR.Compare(
                new IRSubExpr(instr.Left, instr.Right),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            IRFlag.Carry => IR.Compare(
                instr.Left,
                instr.Right,
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitDec(IRDecInstruction instr)
    {
        return _flag switch
        {
            IRFlag.Zero => IR.Compare(
                instr.Target,
                IR.Int(1),
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            IRFlag.Sign => IR.Compare(
                instr.Target,
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitInc(IRIncInstruction instr)
    {
        return _flag switch
        {
            IRFlag.Zero => IR.Compare(
                instr.Target,
                IR.Int(-1),
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            IRFlag.Sign => IR.Compare(
                instr.Target,
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitMul(IRMulInstruction instr)
    {
        return _flag switch
        {
            // Carry flag and Overflow flag in MUL are both set when the high half of the result is non-zero
            // This effectively means the result is too large to fit in the destination register
            IRFlag.Carry or IRFlag.Overflow => IR.Compare(
                new IRMulExpr(instr.Left, instr.Right),
                IR.Uint(0xFFFFFFFF), // Check if result > 32-bit max
                _expectedValue
                    ? IRComparisonType.GreaterThan
                    : IRComparisonType.LessThanOrEqual
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitNeg(IRNegInstruction instr)
    {
        return _flag switch
        {
            // Zero flag: operand == 0
            IRFlag.Zero => IR.Compare(
                instr.Target,
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: result is negative (always true for positive values except 0, always false for negative values)
            IRFlag.Sign => IR.Compare(
                instr.Target,
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.GreaterThan
                    : IRComparisonType.LessThanOrEqual
            ),

            // Carry flag: operand != 0 (CF is set for all non-zero values)
            IRFlag.Carry => IR.Compare(
                instr.Target,
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.NotEqual
                    : IRComparisonType.Equal
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitNot(IRNotInstruction instr)
    {
        return _flag switch
        {
            // Zero flag: ~operand == 0 (which means operand == -1)
            IRFlag.Zero => IR.Compare(
                instr.Operand,
                IR.Int(-1),
                _expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            // Sign flag: result is negative (highest bit is set)
            IRFlag.Sign => IR.Compare(
                new IRNotExpr(instr.Operand),
                IR.Int(0),
                _expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // Carry flag: always cleared to 0 by NOT instruction
            IRFlag.Carry => _expectedValue ?
                IR.False() : // If expected true, never happens
                IR.True(),   // If expected false, always happens

            // Overflow flag: always cleared to 0 by NOT instruction
            IRFlag.Overflow => _expectedValue ?
                IR.False() : // If expected true, never happens
                IR.True(),   // If expected false, always happens

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitOr(IROrInstruction instr)
    {
        return _flag switch
        {
            // Zero flag: result of OR is zero (both operands are zero)
            IRFlag.Zero => new IRLogicalExpr(
                IR.CompareEqual(instr.Left, IR.Int(0)),
                IR.CompareEqual(instr.Right, IR.Int(0)),
                _expectedValue ? IRLogicalOperation.And : IRLogicalOperation.Or),

            // Sign flag: MSB of result is set (result is negative)
            IRFlag.Sign => IR.Compare(
                IR.LogicalOr(instr.Left, instr.Right),
                IR.Int(0),
                _expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitSbb(IRSbbInstruction instr)
    {
        // SBB is complex because it involves the carry flag from a previous operation
        // This is a simplified model focusing on common usage patterns
        return _flag switch
        {
            // Zero flag: result == 0
            IRFlag.Zero => IR.Compare(
                new IRSubExpr(
                    new IRSubExpr(instr.Left, instr.Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: result < 0
            IRFlag.Sign => IR.Compare(
                new IRSubExpr(
                    new IRSubExpr(instr.Left, instr.Right),
                    new IRFlagExpr(IRFlag.Carry)
                ),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Carry flag: (Left < Right) OR (Left == Right AND carry == 1)
            IRFlag.Carry => IR.LogicalOr(
                IR.CompareLessThan(instr.Left, instr.Right),
                IR.LogicalAnd(
                    IR.CompareEqual(instr.Left, instr.Right),
                    new IRFlagExpr(IRFlag.Carry)
                )
            ),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitSub(IRSubInstruction instr)
    {
        return _flag switch
        {
            // Zero flag: left == right
            IRFlag.Zero => IR.Compare(
                instr.Destination,
                instr.Source,
                _expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            // Sign flag: result < 0, which means left < right for signed comparison
            IRFlag.Sign => IR.Compare(
                instr.Destination,
                instr.Source,
                _expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // Carry flag: unsigned overflow (happens when left < right for unsigned comparison)
            IRFlag.Carry => IR.Compare(
                instr.Destination,
                instr.Source,
                _expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitTest(IRTestInstruction instr)
    {
        // Special case for TEST reg, reg (common zero-check pattern)
        if (instr.Left.Equals(instr.Right))
        {
            // For TEST reg, reg, checking Zero flag is equivalent to checking if the register is zero
            if (_flag == IRFlag.Zero)
            {
                return IR.Compare(
                    instr.Left,
                    IR.Int(0),
                    _expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual
                );
            }

            // For TEST reg, reg, checking Sign flag is equivalent to checking if the register is negative
            if (_flag == IRFlag.Sign)
            {
                return IR.Compare(
                    instr.Left,
                    IR.Int(0),
                    _expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual
                );
            }
        }

        // For all other cases, TEST behaves like AND but doesn't modify the operands
        return _flag switch
        {
            IRFlag.Zero => IR.Compare(
                IR.LogicalAnd(instr.Left, instr.Right),
                IR.Int(0),
                _expectedValue ? IRComparisonType.Equal : IRComparisonType.NotEqual),

            IRFlag.Sign => IR.Compare(
                IR.LogicalAnd(instr.Left, instr.Right),
                IR.Int(0),
                _expectedValue ? IRComparisonType.LessThan : IRComparisonType.GreaterThanOrEqual),

            // CF and OF are cleared by TEST
            IRFlag.Carry => _expectedValue ? IR.False() : IR.True(),
            IRFlag.Overflow => _expectedValue ? IR.False() : IR.True(),

            _ => null // Other flags not directly mappable
        };
    }

    public override IRExpression? VisitXor(IRXorInstruction instr)
    {
        return _flag switch
        {
            // Zero flag: result of XOR is zero (means operands are equal)
            IRFlag.Zero => IR.Compare(
                instr.Left,
                instr.Right,
                _expectedValue
                    ? IRComparisonType.Equal
                    : IRComparisonType.NotEqual
            ),

            // Sign flag: MSB of result is set (result is negative)
            IRFlag.Sign => IR.Compare(
                new IRXorExpr(instr.Left, instr.Right),
                IR.Int(0),
                _expectedValue
                    ? IRComparisonType.LessThan
                    : IRComparisonType.GreaterThanOrEqual
            ),

            // Special case: xor reg, reg (clearing a register)
            // This is often used to set the zero flag and clear a register
            _ => null // Other flags not directly mappable
        };
    }
}
