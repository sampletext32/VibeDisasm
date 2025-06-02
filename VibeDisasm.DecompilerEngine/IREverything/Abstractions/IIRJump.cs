using VibeDisasm.DecompilerEngine.IREverything.Expressions;

namespace VibeDisasm.DecompilerEngine.IREverything.Abstractions;

public interface IIRConditionalJump
{
    public IRExpression Target { get; }
    public IRExpression Condition { get; }
}
