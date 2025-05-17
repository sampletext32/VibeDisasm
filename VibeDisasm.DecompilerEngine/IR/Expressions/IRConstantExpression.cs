namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a constant value in the IR
/// </summary>
public class IRConstantExpression : IRExpression
{
    public object Value { get; }
    
    public IRConstantExpression(object value) : base(IRNodeType.Constant)
    {
        Value = value;
    }
    
    public override string ToString() => Value.ToString() ?? string.Empty;
}
