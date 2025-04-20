namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw import descriptor in the PE file (IMAGE_IMPORT_DESCRIPTOR)
/// </summary>
public class RawImportDescriptor : IRawStructure
{
    /// <summary>
    /// RVA to the import lookup table (OriginalFirstThunk)
    /// </summary>
    public uint OriginalFirstThunk { get; set; }

    /// <summary>
    /// Time/date stamp
    /// </summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>
    /// Forwarder chain
    /// </summary>
    public uint ForwarderChain { get; set; }

    /// <summary>
    /// RVA to the DLL name
    /// </summary>
    public uint Name { get; set; }

    /// <summary>
    /// RVA to the import address table (IAT)
    /// </summary>
    public uint FirstThunk { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 20; // Import descriptor is always 20 bytes
}
