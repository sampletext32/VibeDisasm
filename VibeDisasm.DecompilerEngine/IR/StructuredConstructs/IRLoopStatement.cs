using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Base class for all loop constructs in the IR
/// </summary>
public abstract class IRLoopStatement : IRStatement
{
    public IRExpression Condition { get; }
    public IRBlockStatement Body { get; }
    
    protected IRLoopStatement(IRNodeType nodeType, IRExpression condition, IRBlockStatement body) 
        : base(nodeType)
    {
        Condition = condition;
        Body = body;
        
        AddChild(condition);
        AddChild(body);
    }
}
