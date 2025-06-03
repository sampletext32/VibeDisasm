namespace VibeDisasm.Pe.Models;

/// <summary>
/// Represents a collection of resources in a PE file
/// </summary>
public class ResourceInfo
{
    /// <summary>
    /// Gets or sets the list of resources
    /// </summary>
    public List<ResourceEntryInfo> Resources { get; set; } = new();

    /// <summary>
    /// Gets or sets the resource directory RVA
    /// </summary>
    public uint DirectoryRVA { get; set; }

    /// <summary>
    /// Gets or sets the resource directory size
    /// </summary>
    public uint DirectorySize { get; set; }

}
