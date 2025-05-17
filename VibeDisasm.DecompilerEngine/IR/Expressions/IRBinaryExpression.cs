namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a binary operation in the IR
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
