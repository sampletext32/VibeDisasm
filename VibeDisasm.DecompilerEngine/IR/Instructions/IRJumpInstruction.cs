using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a jump instruction in IR.
/// Example: jmp 0x401000 -> IRJumpInstruction(0x401000, false)
/// Example: je 0x401100 -> IRJumpInstruction(0x401100, true)
/// </summary>
public sealed class IRJumpInstruction : IRInstruction
{
    public IRExpression Target { get; init; }
    public bool Conditional { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Target];
    public IRJumpInstruction(IRExpression target, bool conditional)
    {
        Target = target;
        Conditional = conditional;
    }
}
