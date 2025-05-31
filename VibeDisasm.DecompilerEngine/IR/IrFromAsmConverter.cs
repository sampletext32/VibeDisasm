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
        var irBlocks = new List<IRBlock>();

        foreach (var asmBlock in asmFunction.Blocks.Values.OrderBy(x => x.StartAddress))
        {
            var irBlock = new IRBlock
            {
                Id = $"0x{asmBlock.StartAddress:X8}",
                Instructions = asmBlock.Instructions
                    .Select(Lift)
                    .ToList()
            };
            irBlocks.Add(irBlock);
        }

        var irFunction = new IRFunction(
            name: $"Function_0x{asmFunction.Blocks.Values.First(x => x.IsEntryBlock).StartAddress:X8}",
            returnType: IRType.Int,
            parameters: [],
            blocks: irBlocks
        );
        return irFunction;
    }

    private static readonly InstructionToIRVisitor InstructionVisitor = new();

    private static IRInstruction Lift(AsmInstruction instruction)
        => instruction.Accept(InstructionVisitor);
}

