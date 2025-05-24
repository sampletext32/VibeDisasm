namespace VibeDisasm.Disassembler.X86;

public enum AsmJumpType
{
    /// <summary>
    /// The jump is taken
    /// </summary>
    Taken,
    /// <summary>
    /// There is no jump, block switching is direct (one after another)
    /// </summary>
    Fallthrough
}