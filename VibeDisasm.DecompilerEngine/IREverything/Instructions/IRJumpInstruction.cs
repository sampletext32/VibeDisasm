using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.IREverything.Instructions;

/// <summary>
/// Represents a jump instruction in IR.
/// Example: jmp 0x401000 -> IRJumpInstruction(0x401000, null)
/// Example: je 0x401100 -> IRJumpInstruction(0x401100, some_condition)
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public sealed class IRJumpInstruction : IRInstruction
{
    private readonly AsmInstruction _underlyingInstruction;
    public IRExpression Target { get; init; }
    public IRExpression? Condition { get; init; } // null for unconditional
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => Condition is null ? [Target] : [Condition, Target];

    public IRJumpInstruction(AsmInstruction underlyingInstruction, IRExpression target, IRExpression? condition = null)
    {
        _underlyingInstruction = underlyingInstruction;
        Target = target;
        Condition = condition;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitJump(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitJump(this);

    internal override string DebugDisplay => Condition is null
        ? $"IRJumpInstruction(jump -> {Target.DebugDisplay})"
        : $"IRJumpInstruction(jump_if {Condition.DebugDisplay} -> {Target.DebugDisplay})";
}
