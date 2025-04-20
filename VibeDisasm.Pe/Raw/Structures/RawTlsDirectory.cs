namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw TLS directory in the PE file (IMAGE_TLS_DIRECTORY)
/// </summary>
public class RawTlsDirectory : IRawStructure
{
    /// <summary>
    /// Start address of the raw data
    /// </summary>
    public ulong StartAddressOfRawData { get; set; }

    /// <summary>
    /// End address of the raw data
    /// </summary>
    public ulong EndAddressOfRawData { get; set; }

    /// <summary>
    /// Address of the index
    /// </summary>
    public ulong AddressOfIndex { get; set; }

    /// <summary>
    /// Address of the callbacks
    /// </summary>
    public ulong AddressOfCallBacks { get; set; }

    /// <summary>
    /// Size of zero fill
    /// </summary>
    public uint SizeOfZeroFill { get; set; }

    /// <summary>
    /// Characteristics
    /// </summary>
    public uint Characteristics { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 40; // TLS directory is 24 bytes in PE32, 40 bytes in PE32+
}
