namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a constant value in the IR.
/// <para>
/// Constants are literal values that appear in the code. They can be numbers, addresses, or other fixed values.
/// </para>
/// <para>
/// Examples:
/// - Numeric literals: 42, 0x1000, -1
/// - Memory addresses: 0x401000 (as a jump target)
/// - Immediate operands: MOV eax, 5 (the 5 is represented as an IRConstantExpression)
/// </para>
/// <para>
/// In IR form: 42, 0x1000, -1
/// </para>
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
