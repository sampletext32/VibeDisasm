using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents an unconditional jump in the IR
/// </summary>
public class IRJumpStatement : IRStatement
{
    public IRExpression Target { get; }
    
    public IRJumpStatement(IRExpression target) : base(IRNodeType.Jump)
    {
        Target = target;
        AddChild(target);
    }
}
