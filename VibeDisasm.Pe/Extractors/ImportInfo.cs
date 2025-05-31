namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about imports in a PE file
/// </summary>
public class ImportInfo
{
    /// <summary>
    /// Gets or sets the list of imported modules
    /// </summary>
    public List<ImportModuleInfo> Modules { get; set; } = new();
}
