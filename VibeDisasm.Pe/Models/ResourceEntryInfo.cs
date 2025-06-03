namespace VibeDisasm.Pe.Models;

/// <summary>
/// Represents a resource entry in a PE file
/// </summary>
public class ResourceEntryInfo
{
    /// <summary>
    /// Gets or sets the name or ID of the resource
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the resource (if it has an ID instead of a name)
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// Gets or sets whether the resource has a name (as opposed to an ID)
    /// </summary>
    public bool HasName { get; set; }

    /// <summary>
    /// Gets or sets the language ID of the resource
    /// </summary>
    public uint LanguageId { get; set; }

    /// <summary>
    /// Gets or sets the code page of the resource
    /// </summary>
    public uint CodePage { get; set; }

    /// <summary>
    /// Gets or sets the resource type
    /// </summary>
    public ResourceType Type { get; set; }

    /// <summary>
    /// Gets or sets the size of the resource data
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address of the resource data
    /// </summary>
    public uint RVA { get; set; }

    /// <summary>
    /// Gets or sets the absolute file offset of the resource data
    /// </summary>
    public uint FileOffset { get; set; }
}
