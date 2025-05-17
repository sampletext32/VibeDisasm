using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a conditional statement in the IR
/// </summary>
public class IRConditionalStatement : IRStatement
{
    public IRExpression Condition { get; }
    public IRStatement ThenBlock { get; }
    public IRStatement? ElseBlock { get; }
    
    public IRConditionalStatement(IRExpression condition, IRStatement thenBlock, IRStatement? elseBlock = null) 
        : base(IRNodeType.Conditional)
    {
        Condition = condition;
        ThenBlock = thenBlock;
        ElseBlock = elseBlock;
        
        AddChild(condition);
        AddChild(thenBlock);
        
        if (elseBlock != null)
        {
            AddChild(elseBlock);
        }
    }
}
