using VibeDisasm.Disassembler.X86;

namespace VibeDisasm.DecompilerEngine;

public class InstructionBlock
{
    public uint StartAddress { get; set; }

    public List<Instruction> Instructions { get; set; } = [];
}