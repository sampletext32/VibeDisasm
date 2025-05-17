namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a variable reference in the IR.
/// <para>
/// Variable expressions represent references to named variables in the code. These are used
/// to represent local variables, parameters, and special variables like flags and stack.
/// </para>
/// <para>
/// Examples:
/// - Local variables: x, counter, temp
/// - Function parameters: param1, arg2
/// - Special variables: flags (for condition codes), stack (for stack operations)
/// - In decompiled code: int x = 5; (x is represented as an IRVariableExpression)
/// </para>
/// <para>
/// In IR form: x, counter, flags, stack
/// </para>
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
