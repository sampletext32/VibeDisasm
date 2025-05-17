using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a for loop in the IR with initialization, condition, and iteration
/// </summary>
public class IRForLoop : IRLoopStatement
{
    public IRStatement Initialization { get; }
    public IRStatement Iteration { get; }
    
    public IRForLoop(IRStatement initialization, IRExpression condition, IRStatement iteration, IRBlockStatement body) 
        : base(IRNodeType.ForLoop, condition, body)
    {
        Initialization = initialization;
        Iteration = iteration;
        
        AddChild(initialization);
        AddChild(iteration);
    }
}
