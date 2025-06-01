using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.IRAnalyzers;

public class BasicIfThenAnalyzer
{
    public void Handle(IRFunction function, AsmFunction asmFunction)
    {
        var edges = ControlFlowEdgesBuilder.Build(asmFunction);
    }
}
