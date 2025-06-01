using VibeDisasm.DecompilerEngine.IREverything.Instructions;
using VibeDisasm.DecompilerEngine.IREverything.Model;
using VibeDisasm.DecompilerEngine.IREverything.Structuring;
using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine.IREverything;

/// <summary>
/// Converts AsmFunction and CFG to IRFunction and IRBlocks.
/// </summary>
public static class IrFromAsmConverter
{
    public static IRFunction Convert(AsmFunction asmFunction)
    {
        var irBlocks = new List<IRStructuredNode>();

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

        var body = new IRSequenceNode(irBlocks);

        var irFunction = new IRFunction(
            name: $"Function_0x{asmFunction.Blocks.Values.First(x => x.IsEntryBlock).StartAddress:X8}",
            returnType: IRType.Int,
            parameters: [],
            body: body
        );
        return irFunction;
    }

    private static readonly InstructionToIRVisitor InstructionVisitor = new();

    private static IRInstruction Lift(AsmInstruction instruction)
        => instruction.Accept(InstructionVisitor);
}
