using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a switch statement in the IR.
/// <para>
/// A switch statement is a multi-way branch that selects one of several code paths based on the
/// value of an expression. It consists of a selector expression and multiple case blocks, each
/// with a value to match against and a body to execute when matched.
/// </para>
/// <para>
/// Examples:
/// - Simple switch: switch (x) { case 1: doSomething(); break; case 2: doSomethingElse(); break; default: handleDefault(); }
/// - State machine: switch (state) { case STATE_INIT: initialize(); break; case STATE_RUNNING: process(); break; case STATE_DONE: cleanup(); break; }
/// - In assembly: A series of comparisons and conditional jumps, or a jump table
/// </para>
/// <para>
/// In IR form:
/// ```
/// switch (eax) {
///   case 1:
///     ebx = 10;
///     break;
///   case 2:
///     ebx = 20;
///     break;
///   default:
///     ebx = 0;
///     break;
/// }
/// ```
/// </para>
/// </summary>
public class IRSwitchStatement : IRStatement
{
    public class CaseBlock
    {
        public IRExpression Value { get; }
        public IRBlockStatement Body { get; }
        
        public CaseBlock(IRExpression value, IRBlockStatement body)
        {
            Value = value;
            Body = body;
        }
    }
    
    public IRExpression Expression { get; }
    public List<CaseBlock> Cases { get; } = [];
    public IRBlockStatement? DefaultCase { get; set; }
    
    public IRSwitchStatement(IRExpression expression) : base(IRNodeType.Switch)
    {
        Expression = expression;
        AddChild(expression);
    }
    
    public void AddCase(IRExpression value, IRBlockStatement body)
    {
        Cases.Add(new CaseBlock(value, body));
        AddChild(value);
        AddChild(body);
    }
    
    public void SetDefaultCase(IRBlockStatement body)
    {
        DefaultCase = body;
        AddChild(body);
    }
}
