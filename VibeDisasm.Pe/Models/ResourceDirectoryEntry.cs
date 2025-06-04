namespace VibeDisasm.Pe.Models;

/// <summary>
/// Represents a resource directory entry in a PE file's resource hierarchy
/// </summary>
public class ResourceDirectoryEntry
{
    /// <summary>
    /// Gets or sets the name of the resource directory (if named)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the resource directory (if not named)
    /// </summary>
    public ResourceType TypeId { get; set; }

    /// <summary>
    /// Gets or sets whether the resource has a name (as opposed to an ID)
    /// </summary>
    public bool HasName { get; set; }

    /// <summary>
    /// Gets or sets the child directories (next level in the hierarchy)
    /// </summary>
    public List<ResourceDirectoryEntry> Directories { get; set; } = new();

    /// <summary>
    /// Gets or sets the data entries (leaf nodes)
    /// </summary>
    public List<ResourceEntryInfo> DataEntries { get; set; } = new();

    public IEnumerable<ResourceEntryInfo> FlattenResources()
    {
        var rootResources = DataEntries;

        foreach (var resource in rootResources)
        {
            yield return resource;
        }

        foreach (var directory in Directories)
        {
            foreach (var subResource in directory.FlattenResources())
            {
                yield return subResource;
            }
        }
    }
}
