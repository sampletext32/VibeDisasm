using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a variable assignment in the IR
/// </summary>
public class IRAssignmentStatement : IRStatement
{
    public IRExpression Target { get; }
    public IRExpression Value { get; }
    
    public IRAssignmentStatement(IRExpression target, IRExpression value) 
        : base(IRNodeType.Assignment)
    {
        Target = target;
        Value = value;
        
        AddChild(target);
        AddChild(value);
    }
}
