using System.Collections.Generic;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Represents a resource type
/// </summary>
public enum ResourceType
{
    /// <summary>Unknown resource type</summary>
    Unknown = 0,
    /// <summary>Cursor resource</summary>
    Cursor = 1,
    /// <summary>Bitmap resource</summary>
    Bitmap = 2,
    /// <summary>Icon resource</summary>
    Icon = 3,
    /// <summary>Menu resource</summary>
    Menu = 4,
    /// <summary>Dialog resource</summary>
    Dialog = 5,
    /// <summary>String table resource</summary>
    StringTable = 6,
    /// <summary>Font directory resource</summary>
    FontDir = 7,
    /// <summary>Font resource</summary>
    Font = 8,
    /// <summary>Accelerator table resource</summary>
    Accelerator = 9,
    /// <summary>Unformatted resource data</summary>
    RcData = 10,
    /// <summary>Message table resource</summary>
    MessageTable = 11,
    /// <summary>Group cursor resource</summary>
    GroupCursor = 12,
    /// <summary>Group icon resource</summary>
    GroupIcon = 14,
    /// <summary>Version information resource</summary>
    Version = 16,
    /// <summary>Dialog include resource</summary>
    DlgInclude = 17,
    /// <summary>Plug and Play resource</summary>
    PlugPlay = 19,
    /// <summary>VXD resource</summary>
    VXD = 20,
    /// <summary>Animated cursor resource</summary>
    AniCursor = 21,
    /// <summary>Animated icon resource</summary>
    AniIcon = 22,
    /// <summary>HTML resource</summary>
    HTML = 23,
    /// <summary>Manifest resource</summary>
    Manifest = 24
}

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
    /// Gets or sets the raw data of the resource
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();
    
    /// <summary>
    /// Gets or sets the size of the resource data
    /// </summary>
    public uint Size { get; set; }
    
    /// <summary>
    /// Gets or sets the relative virtual address of the resource data
    /// </summary>
    public uint RVA { get; set; }
}

/// <summary>
/// Represents a collection of resources in a PE file
/// </summary>
public class ResourceInfo
{
    /// <summary>
    /// Gets or sets the list of resources
    /// </summary>
    public List<ResourceEntryInfo> Resources { get; set; } = new List<ResourceEntryInfo>();
    
    /// <summary>
    /// Gets or sets the resource directory RVA
    /// </summary>
    public uint DirectoryRVA { get; set; }
    
    /// <summary>
    /// Gets or sets the resource directory size
    /// </summary>
    public uint DirectorySize { get; set; }
}
