using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a function call in the IR
/// </summary>
public class IRCallStatement : IRStatement
{
    public IRExpression Target { get; }
    public List<IRExpression> Arguments { get; }
    
    public IRCallStatement(IRExpression target, List<IRExpression> arguments) 
        : base(IRNodeType.CallStatement)
    {
        Target = target;
        Arguments = arguments;
        
        AddChild(target);
        foreach (var arg in arguments)
        {
            AddChild(arg);
        }
    }
}
