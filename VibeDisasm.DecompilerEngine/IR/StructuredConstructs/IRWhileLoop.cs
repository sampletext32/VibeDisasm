using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a while loop in the IR (condition checked before body).
/// <para>
/// A while loop is a pre-test loop that evaluates its condition before executing the body.
/// If the condition is false initially, the body is never executed.
/// </para>
/// <para>
/// Examples:
/// - Simple counter: while (i < 10) { i++; }
/// - Processing loop: while (ptr != null) { process(ptr); ptr = ptr->next; }
/// - In assembly: A combination of CMP, conditional jumps, and blocks arranged in a loop pattern
/// </para>
/// <para>
/// In IR form:
/// ```
/// while (ecx < 10) {
///   eax = eax + ecx;
///   ecx = ecx + 1;
/// }
/// ```
/// </para>
/// </summary>
public class IRWhileLoop : IRLoopStatement
{
    public IRWhileLoop(IRExpression condition, IRBlockStatement body) 
        : base(IRNodeType.WhileLoop, condition, body)
    {
    }
}
