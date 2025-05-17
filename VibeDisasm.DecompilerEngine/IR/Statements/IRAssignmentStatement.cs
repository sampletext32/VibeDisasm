using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents an assignment operation in the IR.
/// <para>
/// Assignment statements represent operations that store a value into a target location.
/// The target can be a register, memory location, or variable, and the value can be any expression.
/// </para>
/// <para>
/// Examples:
/// - Register assignments: eax = 5, ebx = ecx
/// - Memory assignments: [ebp+8] = eax, [0x401000] = 42
/// - Variable assignments: x = y + z
/// - In x86 instructions: MOV eax, 5 (represented as eax = 5)
/// - In x86 instructions: ADD ebx, ecx (represented as ebx = ebx + ecx)
/// </para>
/// <para>
/// In IR form: eax = 5, [ebp+8] = eax, ebx = (ebx + ecx)
/// </para>
/// </summary>
public class IRAssignmentStatement : IRStatement
{
    public IRExpression Target { get; }
    public IRExpression Value { get; }
    
    public IRAssignmentStatement(IRExpression target, IRExpression value) 
        : base(IRNodeType.Assignment)
    {
        Target = target;
        Value = value;
        
        AddChild(target);
        AddChild(value);
    }
}
