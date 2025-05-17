using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Statements;

namespace VibeDisasm.DecompilerEngine.IR.StructuredConstructs;

/// <summary>
/// Represents a switch statement in the IR
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
