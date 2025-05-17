using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a conditional statement (if-then-else) in the IR.
/// <para>
/// Conditional statements represent branching based on a condition. They consist of a condition
/// expression, a then-block that executes when the condition is true, and an optional else-block
/// that executes when the condition is false.
/// </para>
/// <para>
/// Examples:
/// - Simple if: if (x > 0) { y = 1; }
/// - If-else: if (eax == 0) { ebx = 1; } else { ebx = 2; }
/// - In assembly: A combination of CMP, conditional jumps, and blocks
/// </para>
/// <para>
/// In IR form:
/// ```
/// if (eax > 0) {
///   ebx = 1;
/// } else {
///   ebx = 2;
/// }
/// ```
/// </para>
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
