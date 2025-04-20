namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw base relocation block in the PE file (IMAGE_BASE_RELOCATION)
/// </summary>
public class RawBaseRelocation : IRawStructure
{
    /// <summary>
    /// RVA of the page that contains the fixups
    /// </summary>
    public uint VirtualAddress { get; set; }

    /// <summary>
    /// Total size of the base relocation block
    /// </summary>
    public uint SizeOfBlock { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 8; // Base relocation block header is always 8 bytes
}
