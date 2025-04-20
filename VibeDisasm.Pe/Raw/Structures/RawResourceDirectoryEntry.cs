namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw resource directory entry in the PE file (IMAGE_RESOURCE_DIRECTORY_ENTRY)
/// </summary>
public class RawResourceDirectoryEntry : IRawStructure
{
    /// <summary>
    /// Name or ID of the entry
    /// </summary>
    public uint NameOrId { get; set; }

    /// <summary>
    /// Offset to data or subdirectory
    /// </summary>
    public uint OffsetToData { get; set; }

    /// <summary>
    /// Gets whether this entry has a name (rather than an ID)
    /// </summary>
    public bool IsNamed => (NameOrId & 0x80000000) != 0;

    /// <summary>
    /// Gets the name offset if this entry has a name
    /// </summary>
    public uint NameOffset => NameOrId & 0x7FFFFFFF;

    /// <summary>
    /// Gets the ID if this entry has an ID
    /// </summary>
    public uint Id => NameOrId & 0x7FFFFFFF;

    /// <summary>
    /// Gets whether this entry points to a subdirectory (rather than data)
    /// </summary>
    public bool IsDirectory => (OffsetToData & 0x80000000) != 0;

    /// <summary>
    /// Gets the subdirectory offset if this entry points to a subdirectory
    /// </summary>
    public uint DirectoryOffset => OffsetToData & 0x7FFFFFFF;

    /// <summary>
    /// Gets the data offset if this entry points to data
    /// </summary>
    public uint DataOffset => OffsetToData;

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 8; // Resource directory entry is always 8 bytes
}
