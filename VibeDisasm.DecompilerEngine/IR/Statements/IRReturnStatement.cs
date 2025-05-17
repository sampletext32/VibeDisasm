using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a return statement in the IR
/// </summary>
public class IRReturnStatement : IRStatement
{
    public IRExpression? Value { get; }
    
    public IRReturnStatement(IRExpression? value = null) : base(IRNodeType.Return)
    {
        Value = value;
        
        if (value != null)
        {
            AddChild(value);
        }
    }
}
