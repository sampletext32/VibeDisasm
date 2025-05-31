using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.DecompilerEngine.IR.Visitors;
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
        => (Condition is null ? $"jump -> {Target}" : $"jump_if {Condition} -> {Target}");

    public override void Accept(IIRNodeVisitor visitor) => visitor.Visit(this);

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
