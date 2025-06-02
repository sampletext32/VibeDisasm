using VibeDisasm.DecompilerEngine.IREverything.Expressions;

namespace VibeDisasm.DecompilerEngine.IREverything.Abstractions;

/// <summary>
/// Represents any conditional jump in IR
/// </summary>
public interface IIRConditionalJump
{
    public IRExpression Target { get; }
    public IRExpression Condition { get; }
}
