namespace VibeDisasm.Pe.Models;

/// <summary>
/// Contains information about a delay-loaded import module
/// </summary>
public class DelayImportModuleInfo
{
    /// <summary>
    /// Gets or sets the attributes field from the delay import descriptor
    /// </summary>
    public uint Attributes { get; set; }

    /// <summary>
    /// Gets or sets the name of the DLL
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the module handle RVA
    /// </summary>
    public uint ModuleHandleRva { get; set; }

    /// <summary>
    /// Gets or sets the delay import address table RVA
    /// </summary>
    public uint DelayImportAddressTableRva { get; set; }

    /// <summary>
    /// Gets or sets the delay import name table RVA
    /// </summary>
    public uint DelayImportNameTableRva { get; set; }

    /// <summary>
    /// Gets or sets the bound delay import table RVA
    /// </summary>
    public uint BoundDelayImportTableRva { get; set; }

    /// <summary>
    /// Gets or sets the unload delay import table RVA
    /// </summary>
    public uint UnloadDelayImportTableRva { get; set; }

    /// <summary>
    /// Gets or sets the timestamp field
    /// </summary>
    public uint Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the list of imported functions
    /// </summary>
    public List<DelayImportFunctionInfo> Functions { get; set; } = [];
}
