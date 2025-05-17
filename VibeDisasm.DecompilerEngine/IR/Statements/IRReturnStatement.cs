using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a return statement in the IR.
/// <para>
/// Return statements represent the termination of a function's execution and an optional
/// return value. In assembly, this corresponds to the RET instruction, which may implicitly
/// return a value in a register (typically eax in x86).
/// </para>
/// <para>
/// Examples:
/// - Simple returns: return;
/// - Returns with values: return 42; return x + y;
/// - In x86 instructions: RET (represented as return, with the value typically in eax)
/// - In higher-level code: return eax; (explicit representation of the return value)
/// </para>
/// <para>
/// In IR form: return, return eax
/// </para>
/// </summary>
public class IRReturnStatement : IRStatement
{
    public IRExpression? Value { get; }
    
    public IRReturnStatement(IRExpression? value = null) : base(IRNodeType.Return)
    {
        Value = value;
        
        if (value != null)
        {
            AddChild(value);
        }
    }
}
