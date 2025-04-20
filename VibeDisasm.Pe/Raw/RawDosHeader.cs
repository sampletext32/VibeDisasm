namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Represents the raw DOS header of a PE file
/// </summary>
public class RawDosHeader : IRawStructure
{
    /// <summary>
    /// Magic number ("MZ")
    /// </summary>
    public ushort Magic { get; set; }

    /// <summary>
    /// Bytes on last page of file
    /// </summary>
    public ushort BytesOnLastPage { get; set; }

    /// <summary>
    /// Pages in file
    /// </summary>
    public ushort PagesInFile { get; set; }

    /// <summary>
    /// Relocations
    /// </summary>
    public ushort Relocations { get; set; }

    /// <summary>
    /// Size of header in paragraphs
    /// </summary>
    public ushort SizeOfHeaderInParagraphs { get; set; }

    /// <summary>
    /// Minimum extra paragraphs needed
    /// </summary>
    public ushort MinimumExtraParagraphs { get; set; }

    /// <summary>
    /// Maximum extra paragraphs needed
    /// </summary>
    public ushort MaximumExtraParagraphs { get; set; }

    /// <summary>
    /// Initial (relative) SS value
    /// </summary>
    public ushort InitialSS { get; set; }

    /// <summary>
    /// Initial SP value
    /// </summary>
    public ushort InitialSP { get; set; }

    /// <summary>
    /// Checksum
    /// </summary>
    public ushort Checksum { get; set; }

    /// <summary>
    /// Initial IP value
    /// </summary>
    public ushort InitialIP { get; set; }

    /// <summary>
    /// Initial (relative) CS value
    /// </summary>
    public ushort InitialCS { get; set; }

    /// <summary>
    /// File address of relocation table
    /// </summary>
    public ushort AddressOfRelocationTable { get; set; }

    /// <summary>
    /// Overlay number
    /// </summary>
    public ushort OverlayNumber { get; set; }

    /// <summary>
    /// Reserved words
    /// </summary>
    public ushort[] Reserved1 { get; set; } = new ushort[4];

    /// <summary>
    /// OEM identifier
    /// </summary>
    public ushort OemIdentifier { get; set; }

    /// <summary>
    /// OEM information
    /// </summary>
    public ushort OemInformation { get; set; }

    /// <summary>
    /// Reserved words
    /// </summary>
    public ushort[] Reserved2 { get; set; } = new ushort[10];

    /// <summary>
    /// File address of new exe header (PE header)
    /// </summary>
    public uint AddressOfPeHeader { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 64; // DOS header is always 64 bytes
}
