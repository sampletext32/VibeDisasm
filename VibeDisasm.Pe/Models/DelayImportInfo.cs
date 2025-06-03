namespace VibeDisasm.Pe.Models;

/// <summary>
/// Contains information about delay-loaded imports from a PE file
/// </summary>
public class DelayImportInfo
{
    /// <summary>
    /// Gets or sets the list of delay-loaded import modules
    /// </summary>
    public List<DelayImportModuleInfo> Modules { get; set; } = [];
}
