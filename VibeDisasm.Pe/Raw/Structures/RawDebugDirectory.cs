namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw debug directory entry in the PE file (IMAGE_DEBUG_DIRECTORY)
/// </summary>
public class RawDebugDirectory : IRawStructure
{
    /// <summary>
    /// Reserved, must be zero
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
    /// Type of debug information
    /// </summary>
    public uint Type { get; set; }

    /// <summary>
    /// Size of debug data
    /// </summary>
    public uint SizeOfData { get; set; }

    /// <summary>
    /// Address of debug data
    /// </summary>
    public uint AddressOfRawData { get; set; }

    /// <summary>
    /// Pointer to debug data
    /// </summary>
    public uint PointerToRawData { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 28; // Debug directory entry is always 28 bytes
}
