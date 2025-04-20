namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw resource data entry in the PE file (IMAGE_RESOURCE_DATA_ENTRY)
/// </summary>
public class RawResourceDataEntry : IRawStructure
{
    /// <summary>
    /// RVA to the resource data
    /// </summary>
    public uint OffsetToData { get; set; }

    /// <summary>
    /// Size of the resource data
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// Code page
    /// </summary>
    public uint CodePage { get; set; }

    /// <summary>
    /// Reserved, must be 0
    /// </summary>
    public uint Reserved { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    int IRawStructure.Size => 16; // Resource data entry is always 16 bytes
}
