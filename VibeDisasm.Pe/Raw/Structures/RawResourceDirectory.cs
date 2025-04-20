namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw resource directory in the PE file (IMAGE_RESOURCE_DIRECTORY)
/// </summary>
public class RawResourceDirectory : IRawStructure
{
    /// <summary>
    /// Resource directory characteristics
    /// </summary>
    public uint Characteristics { get; set; }

    /// <summary>
    /// Time/date stamp
    /// </summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>
    /// Major version
    /// </summary>
    public ushort MajorVersion { get; set; }

    /// <summary>
    /// Minor version
    /// </summary>
    public ushort MinorVersion { get; set; }

    /// <summary>
    /// Number of named entries
    /// </summary>
    public ushort NumberOfNamedEntries { get; set; }

    /// <summary>
    /// Number of ID entries
    /// </summary>
    public ushort NumberOfIdEntries { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 16; // Resource directory is always 16 bytes
}
