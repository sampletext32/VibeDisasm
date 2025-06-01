using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Model;

namespace VibeDisasm.DecompilerEngine;

/// <summary>
/// Convenience class for IR creation.
/// </summary>
public static class IR
{
    public static IRAddExpr Add(IRExpression left, IRExpression right) => new(left, right);
    public static IRMulExpr Mul(IRExpression left, IRExpression right) => new(left, right);
    public static IRSubExpr Sub(IRExpression left, IRExpression right) => new(left, right);

    public static IRCompareExpr Compare(IRExpression left, IRExpression right, IRComparisonType comparison) => new(left, right, comparison);
    public static IRCompareExpr CompareEqual(IRExpression left, IRExpression right) => new(left, right, IRComparisonType.Equal);
    public static IRCompareExpr CompareNotEqual(IRExpression left, IRExpression right) => new(left, right, IRComparisonType.NotEqual);
    public static IRCompareExpr CompareLessThan(IRExpression left, IRExpression right) => new(left, right, IRComparisonType.LessThan);
    public static IRCompareExpr CompareGreaterThan(IRExpression left, IRExpression right) => new(left, right, IRComparisonType.GreaterThan);
    public static IRCompareExpr CompareLessThanOrEqual(IRExpression left, IRExpression right) => new(left, right, IRComparisonType.LessThanOrEqual);
    public static IRCompareExpr CompareGreaterThanOrEqual(IRExpression left, IRExpression right) => new(left, right, IRComparisonType.GreaterThanOrEqual);

    public static IRLogicalExpr LogicalAnd(IRExpression left, IRExpression right) => new(left, right, IRLogicalOperation.And);
    public static IRLogicalExpr LogicalOr(IRExpression left, IRExpression right) => new(left, right, IRLogicalOperation.Or);

    public static IRConstantExpr Byte(byte value) => new(value, IRType.Byte);
    public static IRConstantExpr Int(int value) => new(value, IRType.Int);
    public static IRConstantExpr Uint(uint value) => new(value, IRType.Uint);

    public static IRConstantExpr Short(short value) => new(value, IRType.Short);
    public static IRConstantExpr UShort(ushort value) => new(value, IRType.UShort);
    public static IRConstantExpr Long(long value) => new(value, IRType.Long);
    public static IRConstantExpr Ulong(ulong value) => new(value, IRType.Ulong);
    public static IRConstantExpr True() => new(true, IRType.Bool);
    public static IRConstantExpr False() => new(false, IRType.Bool);
    public static IRConstantExpr FromSize(ulong value, int size) => size switch
    {
        8 => Byte((byte)value),
        16 => UShort((ushort)value),
        32 => Uint((uint)value),
        64 => Ulong(value),
        _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
    };
}
