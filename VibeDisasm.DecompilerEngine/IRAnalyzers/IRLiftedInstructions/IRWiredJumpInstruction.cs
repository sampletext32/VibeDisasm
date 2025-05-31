using System.Diagnostics.Contracts;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Visitors;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers.IRLiftedInstructions;

/// <summary>
/// Represents a jump instruction with a reference to the instruction that set its condition flags.
/// </summary>
public class IRWiredJumpInstruction : IRWrappingInstruction<IRJumpInstruction>
{
    public IRInstruction ConditionInstruction { get; set; }

    public IRExpression? Condition => WrappedInstruction.Condition;
    public IRExpression Target => WrappedInstruction.Target;

    public IRWiredJumpInstruction(IRJumpInstruction originalJump, IRInstruction conditionInstruction)
        : base(originalJump)
    {
        ConditionInstruction = conditionInstruction;
    }

    public override string ToString() => ToLangString();

    /// <summary>
    /// Translates the condition to a human-readable string using the context of the condition instruction.
    /// </summary>
    [Pure]
    public string ToLangString()
    {
        if (WrappedInstruction.Condition == null)
        {
            return $"jump to {WrappedInstruction.Target}"; // Unconditional jump
        }

        return WrappedInstruction.ToString();
    }

    public override void Accept(IIRNodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override T Accept<T>(IIRNodeReturningVisitor<T> visitor) => visitor.Visit(this);
}
