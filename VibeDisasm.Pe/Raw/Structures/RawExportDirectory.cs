namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw export directory in the PE file (IMAGE_EXPORT_DIRECTORY)
/// </summary>
public class RawExportDirectory : IRawStructure
{
    /// <summary>
    /// Reserved, must be 0
    /// </summary>
    public uint Characteristics { get; set; }

    /// <summary>
    /// The time/date stamp
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
    /// RVA of the name of the DLL
    /// </summary>
    public uint NameRva { get; set; }

    /// <summary>
    /// The starting ordinal number
    /// </summary>
    public uint OrdinalBase { get; set; }

    /// <summary>
    /// Number of exported functions
    /// </summary>
    public uint NumberOfFunctions { get; set; }

    /// <summary>
    /// Number of exported names
    /// </summary>
    public uint NumberOfNames { get; set; }

    /// <summary>
    /// RVA of the export address table
    /// </summary>
    public uint AddressOfFunctions { get; set; }

    /// <summary>
    /// RVA of the export name pointer table
    /// </summary>
    public uint AddressOfNames { get; set; }

    /// <summary>
    /// RVA of the ordinal table
    /// </summary>
    public uint AddressOfNameOrdinals { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 40; // Export directory is always 40 bytes
}
