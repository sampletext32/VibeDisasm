namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw bound import descriptor in the PE file (IMAGE_BOUND_IMPORT_DESCRIPTOR)
/// </summary>
public class RawBoundImportDescriptor : IRawStructure
{
    /// <summary>
    /// Time/date stamp
    /// </summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>
    /// Offset to the name of the DLL
    /// </summary>
    public ushort OffsetModuleName { get; set; }

    /// <summary>
    /// Number of bound import references
    /// </summary>
    public ushort NumberOfModuleForwarderRefs { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 8; // Bound import descriptor is always 8 bytes
}
