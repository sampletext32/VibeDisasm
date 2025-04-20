using System.Collections.Generic;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about an imported function
/// </summary>
public class ImportFunctionInfo
{
    /// <summary>
    /// Gets or sets the name of the imported function
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the hint value for the imported function
    /// </summary>
    public ushort Hint { get; set; }
    
    /// <summary>
    /// Gets or sets whether the import is by ordinal
    /// </summary>
    public bool IsByOrdinal { get; set; }
    
    /// <summary>
    /// Gets or sets the ordinal value if the import is by ordinal
    /// </summary>
    public ushort Ordinal { get; set; }
    
    /// <summary>
    /// Gets or sets the relative virtual address of the import address table entry
    /// </summary>
    public uint ImportAddressTableRva { get; set; }
}

/// <summary>
/// Contains information about an imported module
/// </summary>
public class ImportModuleInfo
{
    /// <summary>
    /// Gets or sets the name of the imported module
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the list of imported functions from this module
    /// </summary>
    public List<ImportFunctionInfo> Functions { get; set; } = new List<ImportFunctionInfo>();
}

/// <summary>
/// Contains information about imports in a PE file
/// </summary>
public class ImportInfo
{
    /// <summary>
    /// Gets or sets the list of imported modules
    /// </summary>
    public List<ImportModuleInfo> Modules { get; set; } = new List<ImportModuleInfo>();
}
