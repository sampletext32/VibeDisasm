using VibeDisasm.Pe.Raw.Structures;

namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Represents a raw PE file with only the parsed structures and no additional logic
/// </summary>
public class RawPeFile
{

    /// <summary>
    /// The raw file data
    /// </summary>
    public byte[] RawData { get; }

    /// <summary>
    /// The DOS header
    /// </summary>
    public RawDosHeader DosHeader { get; set; } = new RawDosHeader();

    /// <summary>
    /// The PE signature (should be 'PE\0\0')
    /// </summary>
    public uint PeSignature { get; set; }

    /// <summary>
    /// The file header
    /// </summary>
    public RawFileHeader FileHeader { get; set; } = new RawFileHeader();

    /// <summary>
    /// The optional header
    /// </summary>
    public RawOptionalHeader OptionalHeader { get; set; } = new RawOptionalHeader();

    /// <summary>
    /// The section headers
    /// </summary>
    public RawSectionHeader[] SectionHeaders { get; set; } = Array.Empty<RawSectionHeader>();

    // Optional directory structures

    /// <summary>
    /// The export directory
    /// </summary>
    public RawExportDirectory? ExportDirectory { get; set; }

    /// <summary>
    /// The import descriptors
    /// </summary>
    public RawImportDescriptor[]? ImportDescriptors { get; set; }

    /// <summary>
    /// The resource directory
    /// </summary>
    public RawResourceDirectory? ResourceDirectory { get; set; }

    /// <summary>
    /// The exception directory entries
    /// </summary>
    public RawExceptionDirectory[]? ExceptionDirectory { get; set; }

    /// <summary>
    /// The security directory data
    /// </summary>
    public byte[]? SecurityDirectory { get; set; }

    /// <summary>
    /// The base relocation blocks
    /// </summary>
    public RawBaseRelocation[]? BaseRelocationDirectory { get; set; }

    /// <summary>
    /// The debug directory entries
    /// </summary>
    public RawDebugDirectory[]? DebugDirectory { get; set; }

    /// <summary>
    /// The architecture-specific data (unused)
    /// </summary>
    public byte[]? ArchitectureDirectory { get; set; }

    /// <summary>
    /// The global pointer directory (unused)
    /// </summary>
    public uint GlobalPointerDirectory { get; set; }

    /// <summary>
    /// The TLS directory
    /// </summary>
    public RawTlsDirectory? TlsDirectory { get; set; }

    /// <summary>
    /// The load config directory
    /// </summary>
    public RawLoadConfigDirectory? LoadConfigDirectory { get; set; }

    /// <summary>
    /// The bound import descriptors
    /// </summary>
    public RawBoundImportDescriptor[]? BoundImportDirectory { get; set; }

    /// <summary>
    /// The import address table (IAT)
    /// </summary>
    public uint ImportAddressTableDirectory { get; set; }

    /// <summary>
    /// The delay import descriptors
    /// </summary>
    public RawDelayLoadDescriptor[]? DelayImportDirectory { get; set; }

    /// <summary>
    /// The CLR runtime header
    /// </summary>
    public RawClrDirectory? ClrDirectory { get; set; }

    /// <summary>
    /// Initializes a new instance of the RawPeFile class
    /// </summary>
    /// <param name="rawData">The raw file data</param>
    public RawPeFile(byte[] rawData)
    {
        RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
    }

    /// <summary>
    /// Gets whether this is a 64-bit PE file (PE32+)
    /// </summary>
    public bool IsPe32Plus => OptionalHeader.IsPe32Plus;
}
