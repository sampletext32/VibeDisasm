namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a unary operation in the IR.
/// <para>
/// Unary expressions represent operations that take a single operand and produce a result.
/// These include negation, bitwise NOT, logical NOT, increment, and decrement.
/// </para>
/// <para>
/// Examples:
/// - Negation: -a (arithmetic negation)
/// - Bitwise NOT: ~x (invert all bits)
/// - Logical NOT: !condition (boolean negation)
/// - Increment: ++counter (add 1)
/// - Decrement: --counter (subtract 1)
/// - In x86 instructions: NEG eax (represented as eax = -eax)
/// - In x86 instructions: NOT ebx (represented as ebx = ~ebx)
/// - In x86 instructions: INC ecx (represented as ecx = ecx + 1)
/// </para>
/// <para>
/// In IR form: (-eax), (~ebx), (++ecx), (--edx)
/// </para>
/// </summary>
public class IRUnaryExpression : IRExpression
{
    public enum UnaryOperator
    {
        Negate,     // Arithmetic negation (-)
        BitwiseNot, // Bitwise NOT (~)
        LogicalNot, // Logical NOT (!)
        Increment,  // Increment (++)
        Decrement   // Decrement (--)
    }
    
    public UnaryOperator Operator { get; }
    public IRExpression Operand { get; }
    
    public IRUnaryExpression(UnaryOperator op, IRExpression operand) 
        : base(IRNodeType.Unary)
    {
        Operator = op;
        Operand = operand;
        
        AddChild(operand);
    }
    
    public override string ToString()
    {
        string opStr = Operator switch
        {
            UnaryOperator.Negate => "-",
            UnaryOperator.BitwiseNot => "~",
            UnaryOperator.LogicalNot => "!",
            UnaryOperator.Increment => "++",
            UnaryOperator.Decrement => "--",
            _ => "?"
        };
        
        // Handle pre/post increment/decrement differently
        if (Operator == UnaryOperator.Increment || Operator == UnaryOperator.Decrement)
        {
            return $"{opStr}{Operand}"; // Pre-increment/decrement for now
        }
        
        return $"{opStr}({Operand})";
    }
}
