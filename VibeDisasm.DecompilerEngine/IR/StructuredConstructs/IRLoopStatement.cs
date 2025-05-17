using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Base class for all loop constructs in the IR.
/// <para>
/// Loop statements represent repetitive execution of a block of code based on a condition.
/// This is an abstract base class for specific loop types like while, do-while, and for loops.
/// </para>
/// <para>
/// All loop constructs have:
/// - A condition expression that determines whether to continue looping
/// - A body block that contains the statements to execute in each iteration
/// </para>
/// <para>
/// Derived classes include:
/// - IRWhileLoop: Tests the condition before executing the body (pre-test)
/// - IRDoWhileLoop: Tests the condition after executing the body (post-test)
/// - IRForLoop: Includes initialization, condition, and update expressions
/// </para>
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
