using System.Numerics;
using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a constant value in IR.
/// Example: 5 -> IRConstant(5)
/// </summary>
public sealed class IRConstantExpr : IRExpression
{
    public object Value { get; init; }
    public IRType Type { get; init; }
    
    private IRConstantExpr(object value, IRType type)
    {
        Value = value;
        Type = type;
    }

    public static IRConstantExpr Byte(byte value) => new IRConstantExpr(value, IRType.Byte);
    public static IRConstantExpr Int(int value) => new IRConstantExpr(value, IRType.Int);
    public static IRConstantExpr Uint(uint value) => new IRConstantExpr(value, IRType.Uint);

    public static IRConstantExpr Short(short value) => new IRConstantExpr(value, IRType.Short);
    public static IRConstantExpr UShort(ushort value) => new IRConstantExpr(value, IRType.UShort);
    public static IRConstantExpr Long(long value) => new IRConstantExpr(value, IRType.Long);
    public static IRConstantExpr Ulong(ulong value) => new IRConstantExpr(value, IRType.Ulong);
    public static IRConstantExpr Bool(bool value) => new IRConstantExpr(value, IRType.Bool);
    public static IRConstantExpr FromSize(ulong value, int size) => size switch {
        8 => Byte((byte)value),
        16 => UShort((ushort)value),
        32 => Uint((uint)value),
        64 => Ulong(value),
        _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
    };

    public override string ToString()
    {
        switch (Value)
        {
            case byte i:
                return FormatNumber(i);
            case short i:
                return FormatNumber(i);
            case ushort u:
                return FormatNumber(u);
            case int i:
                return FormatNumber(i);
            case uint u:
                return FormatNumber(u);
            case long l:
                return FormatNumber(l);
            case ulong ul:
                return FormatNumber(ul);
            case bool b:
                return b ? "true" : "false";
            default:
                return Value.ToString() ?? "!unknown type constant!";
        }
    }

    private static string FormatNumber(long number)
    {
        long abs = Math.Abs(number);
        string sign = number >= 0 ? "+" : "-";
        string format;

        if (abs == 0)
        {
            format = "X2";
        }
        else if (abs <= 0xFF)
        {
            format = "X2";
        }
        else if (abs <= 0xFFFF)
        {
            format = "X4";
        }
        else
        {
            format = "X8";
        }

        return $"{sign}0x{abs.ToString(format)}";
    }

    private static string FormatNumber(ulong number)
    {
        ulong abs = number;
        string format;

        if (abs == 0)
        {
            format = "X2";
        }
        else if (abs <= 0xFF)
        {
            format = "X2";
        }
        else if (abs <= 0xFFFF)
        {
            format = "X4";
        }
        else
        {
            format = "X8";
        }

        return $"+0x{abs.ToString(format)}";
    }
}
