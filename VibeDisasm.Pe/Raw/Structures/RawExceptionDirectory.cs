namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw exception directory entry in the PE file (RUNTIME_FUNCTION)
/// </summary>
public class RawExceptionDirectory : IRawStructure
{
    /// <summary>
    /// Starting address of the function
    /// </summary>
    public uint BeginAddress { get; set; }

    /// <summary>
    /// Ending address of the function
    /// </summary>
    public uint EndAddress { get; set; }

    /// <summary>
    /// Address of the unwind information
    /// </summary>
    public uint UnwindInfoAddress { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 12; // Exception directory entry is always 12 bytes
}
