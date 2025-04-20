namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw delay load import descriptor in the PE file (IMAGE_DELAYLOAD_DESCRIPTOR)
/// </summary>
public class RawDelayLoadDescriptor : IRawStructure
{
    /// <summary>
    /// Attributes
    /// </summary>
    public uint Attributes { get; set; }

    /// <summary>
    /// RVA to the name of the DLL
    /// </summary>
    public uint DllNameRva { get; set; }

    /// <summary>
    /// RVA to the module handle
    /// </summary>
    public uint ModuleHandleRva { get; set; }

    /// <summary>
    /// RVA to the import address table
    /// </summary>
    public uint ImportAddressTableRva { get; set; }

    /// <summary>
    /// RVA to the import name table
    /// </summary>
    public uint ImportNameTableRva { get; set; }

    /// <summary>
    /// RVA to the bound import address table
    /// </summary>
    public uint BoundImportAddressTableRva { get; set; }

    /// <summary>
    /// RVA to the unload information table
    /// </summary>
    public uint UnloadInformationTableRva { get; set; }

    /// <summary>
    /// Time/date stamp
    /// </summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 32; // Delay load descriptor is always 32 bytes
}
