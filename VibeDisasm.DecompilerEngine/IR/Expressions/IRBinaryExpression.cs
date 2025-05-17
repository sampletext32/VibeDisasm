namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a binary operation in the IR.
/// <para>
/// Binary expressions represent operations that take two operands and produce a result.
/// These include arithmetic operations, bitwise operations, and shifts.
/// </para>
/// <para>
/// Examples:
/// - Arithmetic: a + b, x - y, m * n, p / q
/// - Bitwise: a & b, x | y, m ^ n
/// - Shifts: a << 2, x >> 3
/// - In x86 instructions: ADD eax, ebx (represented as eax = eax + ebx)
/// - In x86 instructions: SHL ecx, 2 (represented as ecx = ecx << 2)
/// </para>
/// <para>
/// In IR form: (eax + ebx), (ecx - 5), (edx & 0xFF), (esi << 2)
/// </para>
/// </summary>
public class IRBinaryExpression : IRExpression
{
    public enum BinaryOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
        LeftShift,
        RightShift
    }
    
    public BinaryOperator Operator { get; }
    public IRExpression Left { get; }
    public IRExpression Right { get; }
    
    public IRBinaryExpression(BinaryOperator op, IRExpression left, IRExpression right) 
        : base(IRNodeType.Binary)
    {
        Operator = op;
        Left = left;
        Right = right;
        
        AddChild(left);
        AddChild(right);
    }
}
