namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about an exported function
/// </summary>
public class ExportFunctionInfo
{
    /// <summary>
    /// Gets or sets the name of the exported function
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ordinal of the exported function
    /// </summary>
    public ushort Ordinal { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address of the exported function
    /// </summary>
    public uint RelativeVirtualAddress { get; set; }

    /// <summary>
    /// Gets or sets whether this is a forwarded export
    /// </summary>
    public bool IsForwarded { get; set; }

    /// <summary>
    /// Gets or sets the forwarded name if this is a forwarded export
    /// </summary>
    public string ForwardedName { get; set; } = string.Empty;
}
