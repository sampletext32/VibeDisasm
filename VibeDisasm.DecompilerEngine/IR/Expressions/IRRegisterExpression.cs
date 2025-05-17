namespace VibeDisasm.DecompilerEngine.IR.Expressions;

/// <summary>
/// Represents a CPU register reference in the IR.
/// <para>
/// Register expressions represent references to CPU registers in the code. These are used
/// when instructions read from or write to registers.
/// </para>
/// <para>
/// Examples:
/// - General purpose registers: eax, ebx, ecx, edx
/// - Special purpose registers: esp (stack pointer), ebp (base pointer)
/// - Segment registers: cs, ds, ss
/// - In x86 instructions: MOV eax, ebx (both eax and ebx are represented as IRRegisterExpression)
/// </para>
/// <para>
/// In IR form: eax, ebx, esp
/// </para>
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
