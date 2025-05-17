using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a for loop in the IR with initialization, condition, and iteration.
/// <para>
/// A for loop is a structured loop with three components:
/// - Initialization: executed once before the loop begins
/// - Condition: evaluated before each iteration to determine whether to continue
/// - Iteration: executed after each iteration of the loop body
/// </para>
/// <para>
/// Examples:
/// - Counting loop: for (int i = 0; i < 10; i++) { sum += i; }
/// - Array traversal: for (int i = 0; i < array.length; i++) { process(array[i]); }
/// - In assembly: A pattern of initialization, condition check, body, iteration, and jump back
/// </para>
/// <para>
/// In IR form:
/// ```
/// for (ecx = 0; ecx < 10; ecx = ecx + 1) {
///   eax = eax + ecx;
/// }
/// ```
/// </para>
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
