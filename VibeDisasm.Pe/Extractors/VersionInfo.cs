using System.Collections.Generic;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Represents version information extracted from a PE file
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// Gets or sets the file version
    /// </summary>
    public string FileVersion { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the product version
    /// </summary>
    public string ProductVersion { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the file flags
    /// </summary>
    public uint FileFlags { get; set; }
    
    /// <summary>
    /// Gets or sets the file OS
    /// </summary>
    public uint FileOS { get; set; }
    
    /// <summary>
    /// Gets or sets the file type
    /// </summary>
    public uint FileType { get; set; }
    
    /// <summary>
    /// Gets or sets the file subtype
    /// </summary>
    public uint FileSubtype { get; set; }
    
    /// <summary>
    /// Gets or sets the language ID
    /// </summary>
    public uint LanguageId { get; set; }
    
    /// <summary>
    /// Gets or sets the code page
    /// </summary>
    public uint CodePage { get; set; }
    
    /// <summary>
    /// Gets or sets the string file info
    /// </summary>
    public Dictionary<string, string> StringFileInfo { get; set; } = new();

    public List<Translation> Translations { get; set; } = [];
}
