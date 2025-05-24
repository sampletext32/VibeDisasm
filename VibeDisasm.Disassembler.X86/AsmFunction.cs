namespace VibeDisasm.Disassembler.X86;

public class AsmFunction
{
    public Dictionary<uint, AsmBlock> Blocks { get; set; } = [];
}