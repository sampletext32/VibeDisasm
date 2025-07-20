namespace VibeDisasm.Pe.Models;

/// <summary>
/// Represents information about a resource directory in a PE file
/// </summary>
public class ResourceDirectoryInfo
{
    /// <summary>
    /// Gets or sets the characteristics of the resource directory
    /// </summary>
    public uint Characteristics { get; set; }

    /// <summary>
    /// Gets or sets the time/date stamp of the resource directory
    /// </summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>
    /// Gets or sets the major version of the resource directory
    /// </summary>
    public ushort MajorVersion { get; set; }

    /// <summary>
    /// Gets or sets the minor version of the resource directory
    /// </summary>
    public ushort MinorVersion { get; set; }

    /// <summary>
    /// Gets or sets the number of named entries in the resource directory
    /// </summary>
    public ushort NumberOfNamedEntries { get; set; }

    /// <summary>
    /// Gets or sets the number of ID entries in the resource directory
    /// </summary>
    public ushort NumberOfIdEntries { get; set; }
}
