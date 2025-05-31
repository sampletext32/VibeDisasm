using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a constant value in IR.
/// Example: 5 -> IRConstant(5)
/// </summary>
public sealed class IRConstantExpr : IRExpression
{
    public object Value { get; init; }
    public IRType Type { get; init; }

    public override List<IRExpression> SubExpressions => [];

    private IRConstantExpr(object value, IRType type)
    {
        Value = value;
        Type = type;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IRConstantExpr other)
        {
            return Value.Equals(other.Value) && Type == other.Type;
        }

        return false;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitConstant(this);

    public static IRConstantExpr Byte(byte value) => new(value, IRType.Byte);
    public static IRConstantExpr Int(int value) => new(value, IRType.Int);
    public static IRConstantExpr Uint(uint value) => new(value, IRType.Uint);

    public static IRConstantExpr Short(short value) => new(value, IRType.Short);
    public static IRConstantExpr UShort(ushort value) => new(value, IRType.UShort);
    public static IRConstantExpr Long(long value) => new(value, IRType.Long);
    public static IRConstantExpr Ulong(ulong value) => new(value, IRType.Ulong);
    public static IRConstantExpr Bool(bool value) => new(value, IRType.Bool);
    public static IRConstantExpr FromSize(ulong value, int size) => size switch
    {
        8 => Byte((byte)value),
        16 => UShort((ushort)value),
        32 => Uint((uint)value),
        64 => Ulong(value),
        _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
    };

    public override string ToString()
    {
        return Value switch
        {
            byte i => FormatNumber(i),
            short i => FormatNumber(i),
            ushort u => FormatNumber(u),
            int i => FormatNumber(i),
            uint u => FormatNumber(u),
            long l => FormatNumber(l),
            ulong ul => FormatNumber(ul),
            bool b => b
                ? "true"
                : "false",
            _ => Value.ToString() ?? "!unknown type constant!"
        };
    }

    private static string FormatNumber(long number)
    {
        var abs = Math.Abs(number);
        var sign = number >= 0 ? "+" : "-";
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
        var abs = number;
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

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
