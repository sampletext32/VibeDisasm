using VibeDisasm.Disassembler.X86;
using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.DecompilerEngine.IR.Expressions;
using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;

namespace VibeDisasm.DecompilerEngine.IR;

/// <summary>
/// Converts AsmFunction and CFG to IRFunction and IRBlocks.
/// </summary>
public static class IrFromAsmConverter
{
    public static IRFunction Convert(AsmFunction asmFunction)
    {
        var edges = ControlFlowEdgesBuilder.Build(asmFunction);
        var blockMap = new Dictionary<uint, IRBlock>();
        var irBlocks = new List<IRBlock>();

        // Map AsmBlocks to IRBlocks
        foreach (var asmBlock in asmFunction.Blocks.Values)
        {
            var irBlock = new IRBlock
            {
                Id = $"0x{asmBlock.StartAddress:X8}",
                Instructions = asmBlock.Instructions
                    .Select(_ => new StubIRInstruction())
                    .ToList()
            };
            blockMap[asmBlock.StartAddress] = irBlock;
            irBlocks.Add(irBlock);
        }

        // Map function
        var irFunction = new IRFunction
        {
            Name = $"Function_0x{asmFunction.Blocks.Keys.FirstOrDefault():X8}",
            ReturnType = new IRType { Name = "int" }, // stub, to be improved
            Parameters = [], // stub, to be improved
            Blocks = irBlocks
        };
        return irFunction;
    }

    private sealed class StubIRInstruction : IRInstruction
    {
        public override IRExpression? Result => null;
        public override IReadOnlyList<IRExpression> Operands => [];
    }
}
