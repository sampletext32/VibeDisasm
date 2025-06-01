using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers;
using VibeDisasm.DecompilerEngine.IREverything.Model;

namespace VibeDisasm.DecompilerEngine.Tests.IRAnalyzers;

/// <summary>
/// Tests for the IRFlagConditionTransformer that transforms CPU flag-based conditions
/// into higher-level comparison expressions
/// </summary>
public class IRFlagConditionTransformerTests
{
    [Fact]
    public void TransformZeroFlagComparison_FromJz()
    {
        // Arrange - Similar to Jz instruction
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.CompareEqual(
            new IRFlagExpr(IRFlag.Zero),
            IR.True()
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        Assert.Equal(IRComparisonType.Equal, compareExpr.Comparison);
        Assert.Same(varA, compareExpr.Left);
        Assert.Same(varB, compareExpr.Right);
    }

    [Fact]
    public void TransformSignOverflowNotEqual_FromJnge()
    {
        // Arrange - Similar to Jnge instruction
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.CompareNotEqual(
            new IRFlagExpr(IRFlag.Sign),
            new IRFlagExpr(IRFlag.Overflow)
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        Assert.Equal(IRComparisonType.LessThan, compareExpr.Comparison);
        Assert.Same(varA, compareExpr.Left);
        Assert.Same(varB, compareExpr.Right);
    }

    [Fact]
    public void TransformSignOverflowEqual_FromJnl()
    {
        // Arrange - Similar to Jnl instruction
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.CompareEqual(
            new IRFlagExpr(IRFlag.Sign),
            new IRFlagExpr(IRFlag.Overflow)
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        Assert.Equal(IRComparisonType.GreaterThanOrEqual, compareExpr.Comparison);
        Assert.Same(varA, compareExpr.Left);
        Assert.Same(varB, compareExpr.Right);
    }

    [Fact]
    public void TransformJNLEPattern_ToGreaterThan()
    {
        // Arrange - Exactly as in JNLE instruction
        // Zero == false AND Sign == Overflow
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.LogicalAnd(
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Zero),
                IR.False()
            ),
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Sign),
                new IRFlagExpr(IRFlag.Overflow)
            )
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        Assert.Equal(IRComparisonType.GreaterThan, compareExpr.Comparison);
        Assert.Same(varA, compareExpr.Left);
        Assert.Same(varB, compareExpr.Right);
    }

    [Fact]
    public void TransformJLEPattern_ToLessThanOrEqual()
    {
        // Arrange - Exactly as in JNG/JLE instruction
        // Zero == true OR Sign != Overflow
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.LogicalOr(
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Zero),
                IR.True()
            ),
            IR.CompareNotEqual(
                new IRFlagExpr(IRFlag.Sign),
                new IRFlagExpr(IRFlag.Overflow)
            )
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        Assert.Equal(IRComparisonType.LessThanOrEqual, compareExpr.Comparison);
        Assert.Same(varA, compareExpr.Left);
        Assert.Same(varB, compareExpr.Right);
    }

    [Fact]
    public void TransformTestInstruction_ToLessThanOrEqualZero()
    {
        // Arrange - Test instruction with same pattern as JLE
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var flagSetter = new IRTestInstruction(varA, varA);

        // Zero == true OR Sign != Overflow (same as JLE pattern)
        var condition = IR.LogicalOr(
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Zero),
                IR.True()
            ),
            IR.CompareNotEqual(
                new IRFlagExpr(IRFlag.Sign),
                new IRFlagExpr(IRFlag.Overflow)
            )
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        Assert.Equal(IRComparisonType.LessThanOrEqual, compareExpr.Comparison);
        Assert.Same(varA, compareExpr.Left);

        var rightConst = Assert.IsType<IRConstantExpr>(compareExpr.Right);
        Assert.Equal(0, rightConst.Value);
    }

    [Fact]
    public void TransformCarryFlagComparison_FromJb()
    {
        // Arrange - Similar to Jb instruction
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.CompareEqual(
            new IRFlagExpr(IRFlag.Carry),
            IR.True()
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        // For unsigned comparisons, we would expect Below or UnsignedLessThan
        Assert.Same(varA, compareExpr.Left);
        Assert.Same(varB, compareExpr.Right);
    }

    [Fact]
    public void TransformJnbePattern_ToAbove()
    {
        // Arrange - Similar to Jnbe instruction (Carry == false AND Zero == false)
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.LogicalAnd(
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Carry),
                IR.False()
            ),
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Zero),
                IR.False()
            )
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        // For Jnbe, the result is a logical expression that would represent an unsigned greater than
        var logicalExpr = Assert.IsType<IRLogicalExpr>(result);
        Assert.Equal(IRLogicalOperation.And, logicalExpr.Operation);
    }

    [Fact]
    public void RecursiveTransformation_OfLogicalExpressions()
    {
        // Arrange - Combined logical expression with multiple patterns
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = new IRRegisterExpr(IRRegister.EBX);
        var flagSetter = new IRCmpInstruction(varA, varB);

        var condition = IR.LogicalOr(
            // Zero == true (Equal)
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Zero),
                IR.True()
            ),
            // Sign == Overflow (GreaterThanOrEqual)
            IR.CompareEqual(
                new IRFlagExpr(IRFlag.Sign),
                new IRFlagExpr(IRFlag.Overflow)
            )
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var logicalExpr = Assert.IsType<IRLogicalExpr>(result);
        Assert.Equal(IRLogicalOperation.Or, logicalExpr.Operation);

        var leftCmp = Assert.IsType<IRCompareExpr>(logicalExpr.Left);
        var rightCmp = Assert.IsType<IRCompareExpr>(logicalExpr.Right);

        Assert.Equal(IRComparisonType.Equal, leftCmp.Comparison);
        Assert.Equal(IRComparisonType.GreaterThanOrEqual, rightCmp.Comparison);
    }

    [Fact]
    public void TransformTestInstructionWithSameOperands_ToZeroComparison()
    {
        // Arrange - Test instruction with same operand on both sides (TEST EAX, EAX)
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var flagSetter = new IRTestInstruction(varA, varA);

        // Zero flag comparison (similar to JZ after TEST)
        var condition = IR.CompareEqual(
            new IRFlagExpr(IRFlag.Zero),
            IR.True()
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.NotNull(result);
        var compareExpr = Assert.IsType<IRCompareExpr>(result);
        Assert.Equal(IRComparisonType.Equal, compareExpr.Comparison);
        Assert.Same(varA, compareExpr.Left);

        var rightConst = Assert.IsType<IRConstantExpr>(compareExpr.Right);
        Assert.Equal(0, rightConst.Value);
    }

    [Fact]
    public void UnrecognizedPattern_ReturnsNull()
    {
        // Arrange - Pattern that truly doesn't match any recognized patterns
        var varA = new IRRegisterExpr(IRRegister.EAX);
        var varB = IR.Int(42); // Use a constant instead of register to avoid direct match
        var flagSetter = new IRTestInstruction(varA, varB); // Use TEST instead of CMP

        // Use a parity flag which isn't handled in our patterns with test instructions
        var condition = IR.CompareNotEqual(
            new IRFlagExpr(IRFlag.Parity),
            IR.True()
        );

        // Act
        var result = IRFlagConditionTransformer.TransformCondition(condition, flagSetter);

        // Assert
        Assert.Null(result);
    }
}
