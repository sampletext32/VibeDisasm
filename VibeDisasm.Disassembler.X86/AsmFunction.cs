namespace VibeDisasm.Disassembler.X86;

/// <summary>Represents a function in the disassembled code.</summary>
public class AsmFunction
{
    /// <summary>Basic blocks in the function, keyed by their start address.</summary>
    public Dictionary<uint, AsmBlock> Blocks { get; set; } = [];
}
