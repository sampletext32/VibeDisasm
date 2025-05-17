using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a do-while loop in the IR (condition checked after body)
/// </summary>
public class IRDoWhileLoop : IRLoopStatement
{
    public IRDoWhileLoop(IRExpression condition, IRBlockStatement body) 
        : base(IRNodeType.DoWhileLoop, condition, body)
    {
    }
}
