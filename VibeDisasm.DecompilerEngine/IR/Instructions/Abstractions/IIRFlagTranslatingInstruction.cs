using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR.Instructions.Abstractions;

public interface IIRFlagTranslatingInstruction
{
    IRExpression? GetFlagCondition(IRFlag flag, bool expectedValue);
}
