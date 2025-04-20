namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Represents the raw file header of a PE file (IMAGE_FILE_HEADER)
/// </summary>
public class RawFileHeader : IRawStructure
{
    /// <summary>
    /// Machine type
    /// </summary>
    public ushort Machine { get; set; }

    /// <summary>
    /// Number of sections
    /// </summary>
    public ushort NumberOfSections { get; set; }

    /// <summary>
    /// Time date stamp
    /// </summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>
    /// Pointer to symbol table
    /// </summary>
    public uint PointerToSymbolTable { get; set; }

    /// <summary>
    /// Number of symbols
    /// </summary>
    public uint NumberOfSymbols { get; set; }

    /// <summary>
    /// Size of optional header
    /// </summary>
    public ushort SizeOfOptionalHeader { get; set; }

    /// <summary>
    /// Characteristics
    /// </summary>
    public ushort Characteristics { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 20; // File header is always 20 bytes
}
