using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Abstractions;
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
public sealed class IRConditionalJumpInstruction : IRInstruction, IIRConditionalJump
{
    private readonly AsmInstruction _underlyingInstruction;
    public IRExpression Target { get; init; }
    public IRExpression Condition { get; init; }
    public override IRExpression? Result => null;
    public override IReadOnlyList<IRExpression> Operands => [Condition, Target];

    public IRConditionalJumpInstruction(AsmInstruction underlyingInstruction, IRExpression target, IRExpression condition)
    {
        _underlyingInstruction = underlyingInstruction;
        Target = target;
        Condition = condition;
    }

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitConditionalJump(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitConditionalJump(this);

    internal override string DebugDisplay => $"IRConditionalJumpInstruction(jump_if {Condition.DebugDisplay} -> {Target.DebugDisplay})";
}
