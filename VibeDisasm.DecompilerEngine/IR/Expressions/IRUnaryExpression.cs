namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a unary operation in the IR
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
