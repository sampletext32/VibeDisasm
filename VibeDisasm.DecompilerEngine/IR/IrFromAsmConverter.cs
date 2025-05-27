using VibeDisasm.Disassembler.X86;
using VibeDisasm.DecompilerEngine.ControlFlow;
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

        foreach (var asmBlock in asmFunction.Blocks.Values)
        {
            var irBlock = new IRBlock
            {
                Id = $"0x{asmBlock.StartAddress:X8}",
                Instructions = asmBlock.Instructions
                    .Select(Lift)
                    .ToList()
            };
            blockMap[asmBlock.StartAddress] = irBlock;
            irBlocks.Add(irBlock);
        }

        var irFunction = new IRFunction
        {
            Name = $"Function_0x{asmFunction.Blocks.Keys.FirstOrDefault():X8}",
            ReturnType = new IRType { Name = "int" },
            Parameters = [],
            Blocks = irBlocks
        };
        return irFunction;
    }

    private static readonly InstructionToIRVisitor InstructionVisitor = new();

    private static IRInstruction Lift(AsmInstruction instruction)
        => instruction.Accept(InstructionVisitor);
}

