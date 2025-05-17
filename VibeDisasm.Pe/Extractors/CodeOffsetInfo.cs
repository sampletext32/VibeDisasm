namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about a definite code offset in a PE file
/// </summary>
public class CodeOffsetInfo
{
    /// <summary>
    /// Gets or sets the file offset of the code
    /// </summary>
    public uint FileOffset { get; set; }
    
    /// <summary>
    /// Gets or sets the relative virtual address (RVA) of the code
    /// </summary>
    public uint RelativeVirtualAddress { get; set; }
    
    /// <summary>
    /// Gets or sets the source of the code identification (e.g., "Entry Point", "Export", etc.)
    /// </summary>
    public string Source { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets a description of the code
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
