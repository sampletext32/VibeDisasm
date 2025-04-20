using System.Collections.Generic;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about an exported function
/// </summary>
public class ExportFunctionInfo
{
    /// <summary>
    /// Gets or sets the name of the exported function
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the ordinal of the exported function
    /// </summary>
    public ushort Ordinal { get; set; }
    
    /// <summary>
    /// Gets or sets the relative virtual address of the exported function
    /// </summary>
    public uint RelativeVirtualAddress { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is a forwarded export
    /// </summary>
    public bool IsForwarded { get; set; }
    
    /// <summary>
    /// Gets or sets the forwarded name if this is a forwarded export
    /// </summary>
    public string ForwardedName { get; set; } = string.Empty;
}

/// <summary>
/// Contains information about exports from a PE file
/// </summary>
public class ExportInfo
{
    /// <summary>
    /// Gets or sets the name of the DLL
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the base ordinal for exports
    /// </summary>
    public uint OrdinalBase { get; set; }
    
    /// <summary>
    /// Gets or sets the time/date stamp of the export directory
    /// </summary>
    public uint TimeDateStamp { get; set; }
    
    /// <summary>
    /// Gets or sets the major version of the export directory
    /// </summary>
    public ushort MajorVersion { get; set; }
    
    /// <summary>
    /// Gets or sets the minor version of the export directory
    /// </summary>
    public ushort MinorVersion { get; set; }
    
    /// <summary>
    /// Gets or sets the list of exported functions
    /// </summary>
    public List<ExportFunctionInfo> Functions { get; set; } = new List<ExportFunctionInfo>();
}
