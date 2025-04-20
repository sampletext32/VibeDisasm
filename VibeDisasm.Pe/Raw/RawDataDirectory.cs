namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Represents a raw data directory entry in the PE file
/// </summary>
public class RawDataDirectory : IRawStructure
{
    /// <summary>
    /// Virtual address of the table
    /// </summary>
    public uint VirtualAddress { get; set; }

    /// <summary>
    /// Size of the table
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    int IRawStructure.Size => 8; // Data directory is always 8 bytes
}
