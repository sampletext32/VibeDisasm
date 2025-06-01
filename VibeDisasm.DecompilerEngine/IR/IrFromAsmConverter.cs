using VibeDisasm.DecompilerEngine.IR.Instructions;
using VibeDisasm.DecompilerEngine.IR.Model;
using VibeDisasm.Disassembler.X86;

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
            var irBlock = new IRBlock(
                asmBlock.StartAddress,
                asmBlock.Instructions
                    .Select(Lift)
                    .ToList(),
                asmBlock.IsEntryBlock
            );
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
