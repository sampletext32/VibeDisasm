using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Statements;

/// <summary>
/// Represents an unconditional jump in the IR.
/// <para>
/// Jump statements represent direct transfers of control to another location in the code.
/// The target is typically an address or a label representing a basic block.
/// </para>
/// <para>
/// Examples:
/// - Direct jumps: goto 0x401000, goto label1
/// - In x86 instructions: JMP 0x401000 (represented as goto 0x401000)
/// - In x86 instructions: JZ 0x401020 (represented as a conditional jump, which is a different class)
/// </para>
/// <para>
/// In IR form: goto 0x401000, goto Block_0x401000
/// </para>
/// </summary>
public class IRJumpStatement : IRStatement
{
    public IRExpression Target { get; }
    
    public IRJumpStatement(IRExpression target) : base(IRNodeType.Jump)
    {
        Target = target;
        AddChild(target);
    }
}
