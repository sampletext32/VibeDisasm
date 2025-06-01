using System.Diagnostics;
using VibeDisasm.DecompilerEngine.IREverything.Expressions;
using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.Visitors;

namespace VibeDisasm.DecompilerEngine.IREverything.IRAnalyzers.IRLiftedInstructions;

/// <summary>
/// Represents a jump instruction with a reference to the instruction that set its condition flags.
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class IRWiredJumpInstruction : IRWrappingInstruction<IRJumpInstruction>
{
    public IRInstruction ConditionInstruction { get; set; }

    public IRExpression? Condition => WrappedInstruction.Condition;
    public IRExpression Target => WrappedInstruction.Target;

    public IRWiredJumpInstruction(IRJumpInstruction originalJump, IRInstruction conditionInstruction)
        : base(originalJump) =>
        ConditionInstruction = conditionInstruction;

    public override void Accept(IIRNodeVisitor visitor) => visitor.VisitWiredJump(this);

    public override T? Accept<T>(IIRNodeReturningVisitor<T> visitor) where T : default => visitor.VisitWiredJump(this);

    internal override string DebugDisplay => $"IRWiredJumpInstruction(Condition: {ConditionInstruction.DebugDisplay} Jump: {WrappedInstruction.DebugDisplay})";
}
