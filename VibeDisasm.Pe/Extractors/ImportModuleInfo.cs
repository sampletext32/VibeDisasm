namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about an imported module
/// </summary>
public class ImportModuleInfo
{
    /// <summary>
    /// Gets or sets the name of the imported module
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of imported functions from this module
    /// </summary>
    public List<ImportFunctionInfo> Functions { get; set; } = new();
}
