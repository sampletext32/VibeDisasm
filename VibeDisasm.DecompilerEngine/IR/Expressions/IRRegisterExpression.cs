namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a CPU register reference in the IR
/// </summary>
public class IRRegisterExpression : IRExpression
{
    public string RegisterName { get; }
    
    public IRRegisterExpression(string registerName) : base(IRNodeType.Register)
    {
        RegisterName = registerName;
    }
    
    public override string ToString() => RegisterName;
}
