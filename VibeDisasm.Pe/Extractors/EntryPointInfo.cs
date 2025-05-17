namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Information about an entry point
/// </summary>
public class EntryPointInfo
{
    /// <summary>
    /// File offset of the entry point
    /// </summary>
    public uint FileOffset { get; }
    
    /// <summary>
    /// Relative virtual address of the entry point
    /// </summary>
    public uint RVA { get; }
    
    /// <summary>
    /// Source of the entry point (e.g., "Entry Point", "Export")
    /// </summary>
    public string Source { get; }
    
    /// <summary>
    /// Description of the entry point
    /// </summary>
    public string Description { get; }

    public string ComputedView { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EntryPointInfo(uint fileOffset, uint rva, string source, string description)
    {
        FileOffset = fileOffset;
        RVA = rva;
        Source = source;
        Description = description;

        ComputedView = $"{RVA:X8} - {Source}: {Description}";
    }
}