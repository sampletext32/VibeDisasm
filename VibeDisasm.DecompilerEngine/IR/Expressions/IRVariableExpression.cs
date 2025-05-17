namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a variable reference in the IR
/// </summary>
public class IRVariableExpression : IRExpression
{
    public string Name { get; }
    
    public IRVariableExpression(string name) : base(IRNodeType.Variable)
    {
        Name = name;
    }
    
    public override string ToString() => Name;
}
