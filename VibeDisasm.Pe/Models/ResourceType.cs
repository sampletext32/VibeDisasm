namespace VibeDisasm.Pe.Models;

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
