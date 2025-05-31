using VibeDisasm.DecompilerEngine.IR.Expressions;

namespace VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;

public interface IIRFlagTranslatingInstruction
{
    IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue);
}