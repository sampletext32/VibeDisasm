using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a while loop in the IR (condition checked before body)
/// </summary>
public class IRWhileLoop : IRLoopStatement
{
    public IRWhileLoop(IRExpression condition, IRBlockStatement body) 
        : base(IRNodeType.WhileLoop, condition, body)
    {
    }
}
