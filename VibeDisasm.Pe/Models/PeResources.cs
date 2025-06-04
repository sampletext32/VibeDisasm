namespace VibeDisasm.Pe.Models;

/// <summary>
/// Represents a collection of resources in a PE file
/// </summary>
public class PeResources
{
    public uint Characteristics { get; set; }
    public uint TimeDateStamp { get; set; }
    public ushort MajorVersion { get; set; }
    public ushort MinorVersion { get; set; }
    public ushort NamedEntries { get; set; }
    public ushort IdEntries { get; set; }

    /// <summary>
    /// Gets or sets the root directory of the resource hierarchy
    /// </summary>
    public List<ResourceDirectoryEntry> RootDirectory { get; set; } = new();

    /// <summary>
    /// Gets or sets the resource directory RVA
    /// </summary>
    public uint DirectoryRva { get; set; }

    /// <summary>
    /// Gets or sets the resource directory size
    /// </summary>
    public uint DirectorySize { get; set; }

    public IEnumerable<ResourceEntryInfo> FlattenResources() => RootDirectory.SelectMany(x => x.FlattenResources());
}
