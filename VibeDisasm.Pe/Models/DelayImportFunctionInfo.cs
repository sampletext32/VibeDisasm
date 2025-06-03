namespace VibeDisasm.Pe.Models;

/// <summary>
/// Contains information about a delay-loaded import function
/// </summary>
public class DelayImportFunctionInfo
{
    /// <summary>
    /// Gets or sets the ordinal or hint value
    /// </summary>
    public ushort Ordinal { get; set; }

    /// <summary>
    /// Gets or sets the name of the function
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the function is imported by ordinal
    /// </summary>
    public bool ImportByOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address (RVA) of the import address table entry
    /// </summary>
    public uint ImportAddressTableRva { get; set; }

    /// <summary>
    /// Gets or sets the file offset of the import address table entry
    /// </summary>
    public uint ImportAddressTableOffset { get; set; }
}
