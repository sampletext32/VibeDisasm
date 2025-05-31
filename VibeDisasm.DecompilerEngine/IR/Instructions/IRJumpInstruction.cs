using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.IR.Instructions;

/// <summary>
/// Represents a jump instruction in IR.
/// Example: jmp 0x401000 -> IRJumpInstruction(0x401000, null)
/// Example: je 0x401100 -> IRJumpInstruction(0x401100, some_condition)
/// </summary>
public sealed class IRJumpInstruction : IRInstruction
{
    private readonly AsmInstruction _underlyingInstruction;
    public IRExpression Target { get; init; }
    public IRExpression? Condition { get; init; } // null for unconditional
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => Condition is null ? [Target] : [Condition, Target];
    
    // Jump instructions don't modify flags in x86
    public override IReadOnlyList<IRFlagEffect> SideEffects => [];

    public IRJumpInstruction(AsmInstruction underlyingInstruction, IRExpression target, IRExpression? condition = null)
    {
        _underlyingInstruction = underlyingInstruction;
        Target = target;
        Condition = condition;
    }

    public override string ToString()
        => $"{_underlyingInstruction.Address:X8} " + (Condition is null ? $"jump -> {Target}" : $"jump_if {Condition} -> {Target}");
}
