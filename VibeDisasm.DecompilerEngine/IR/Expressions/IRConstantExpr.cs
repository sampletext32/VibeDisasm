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

    public static IRConstantExpr Int(int value) => new IRConstantExpr(value, IRType.Int);
    public static IRConstantExpr Uint(uint value) => new IRConstantExpr(value, IRType.Uint);
    public static IRConstantExpr Long(long value) => new IRConstantExpr(value, IRType.Long);
    public static IRConstantExpr Ulong(ulong value) => new IRConstantExpr(value, IRType.Ulong);
    public static IRConstantExpr Bool(bool value) => new IRConstantExpr(value, IRType.Bool);

    public override string ToString()
    {
        switch (Value)
        {
            case int i:
                return i.ToString("X8");
            case uint u:
                return u.ToString("X8");
            case long l:
                return l.ToString("X8");
            case ulong ul:
                return ul.ToString("X8");
            case bool b:
                return b ? "true" : "false";
            default:
                return Value.ToString() ?? "!unknown type constant!";
        }
    }
}
