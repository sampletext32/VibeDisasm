using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a do-while loop in the IR (condition checked after body).
/// <para>
/// A do-while loop is a post-test loop that evaluates its condition after executing the body.
/// The body is always executed at least once, even if the condition is initially false.
/// </para>
/// <para>
/// Examples:
/// - Input validation: do { input = getInput(); } while (input < 0);
/// - Processing with guaranteed first iteration: do { process(data); } while (moreDataAvailable());
/// - In assembly: A block of code followed by a conditional jump back to the beginning
/// </para>
/// <para>
/// In IR form:
/// ```
/// do {
///   eax = getInput();
///   process(eax);
/// } while (eax != 0);
/// ```
/// </para>
/// </summary>
public class IRDoWhileLoop : IRLoopStatement
{
    public IRDoWhileLoop(IRExpression condition, IRBlockStatement body) 
        : base(IRNodeType.DoWhileLoop, condition, body)
    {
    }
}
