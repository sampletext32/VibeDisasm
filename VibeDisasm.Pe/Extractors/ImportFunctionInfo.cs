namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about an imported function
/// </summary>
public class ImportFunctionInfo
{
    /// <summary>
    /// Gets or sets the name of the imported function
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hint value for the imported function
    /// </summary>
    public ushort Hint { get; set; }

    /// <summary>
    /// Gets or sets whether the import is by ordinal
    /// </summary>
    public bool IsByOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the ordinal value if the import is by ordinal
    /// </summary>
    public ushort Ordinal { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address of the import address table entry
    /// </summary>
    public uint ImportAddressTableRva { get; set; }
}
