namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw CLR header in the PE file (IMAGE_COR20_HEADER)
/// </summary>
public class RawClrDirectory : IRawStructure
{
    /// <summary>
    /// Size of the header
    /// </summary>
    public uint HeaderSize { get; set; }

    /// <summary>
    /// Major runtime version
    /// </summary>
    public ushort MajorRuntimeVersion { get; set; }

    /// <summary>
    /// Minor runtime version
    /// </summary>
    public ushort MinorRuntimeVersion { get; set; }

    /// <summary>
    /// Metadata RVA
    /// </summary>
    public uint MetadataRva { get; set; }

    /// <summary>
    /// Metadata size
    /// </summary>
    public uint MetadataSize { get; set; }

    /// <summary>
    /// Flags
    /// </summary>
    public uint Flags { get; set; }

    /// <summary>
    /// Entry point token or RVA
    /// </summary>
    public uint EntryPointTokenOrRva { get; set; }

    /// <summary>
    /// Resources RVA
    /// </summary>
    public uint ResourcesRva { get; set; }

    /// <summary>
    /// Resources size
    /// </summary>
    public uint ResourcesSize { get; set; }

    /// <summary>
    /// Strong name signature RVA
    /// </summary>
    public uint StrongNameSignatureRva { get; set; }

    /// <summary>
    /// Strong name signature size
    /// </summary>
    public uint StrongNameSignatureSize { get; set; }

    /// <summary>
    /// Code manager table RVA
    /// </summary>
    public uint CodeManagerTableRva { get; set; }

    /// <summary>
    /// Code manager table size
    /// </summary>
    public uint CodeManagerTableSize { get; set; }

    /// <summary>
    /// VTable fixups RVA
    /// </summary>
    public uint VTableFixupsRva { get; set; }

    /// <summary>
    /// VTable fixups size
    /// </summary>
    public uint VTableFixupsSize { get; set; }

    /// <summary>
    /// Export address table jumps RVA
    /// </summary>
    public uint ExportAddressTableJumpsRva { get; set; }

    /// <summary>
    /// Export address table jumps size
    /// </summary>
    public uint ExportAddressTableJumpsSize { get; set; }

    /// <summary>
    /// Managed native header RVA
    /// </summary>
    public uint ManagedNativeHeaderRva { get; set; }

    /// <summary>
    /// Managed native header size
    /// </summary>
    public uint ManagedNativeHeaderSize { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 72; // CLR header is always 72 bytes
}
