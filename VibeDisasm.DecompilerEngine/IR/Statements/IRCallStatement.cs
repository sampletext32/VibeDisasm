using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents a function call in the IR.
/// <para>
/// Call statements represent invocations of functions or subroutines. They include
/// the target function (address or name) and any arguments passed to the function.
/// </para>
/// <para>
/// Examples:
/// - Direct function calls: call printf, call 0x401000
/// - Function calls with arguments: printf("Hello, %s", name), memcpy(dest, src, size)
/// - In x86 instructions: CALL printf (represented as call printf)
/// - In x86 instructions: CALL [eax] (represented as call [eax], calling the function whose address is in eax)
/// </para>
/// <para>
/// In IR form: call printf, call 0x401000, call [eax]
/// </para>
/// </summary>
public class IRCallStatement : IRStatement
{
    public IRExpression Target { get; }
    public List<IRExpression> Arguments { get; }
    
    public IRCallStatement(IRExpression target, List<IRExpression> arguments) 
        : base(IRNodeType.Call)
    {
        Target = target;
        Arguments = arguments;
        
        AddChild(target);
        foreach (var arg in arguments)
        {
            AddChild(arg);
        }
    }
}
