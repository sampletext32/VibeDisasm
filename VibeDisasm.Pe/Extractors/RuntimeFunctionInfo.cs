namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about a runtime function entry in the exception directory
/// </summary>
public class RuntimeFunctionInfo
{
    /// <summary>
    /// Gets or sets the index of the function in the exception directory
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address (RVA) of the function start
    /// </summary>
    public uint BeginAddress { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address (RVA) of the function end
    /// </summary>
    public uint EndAddress { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address (RVA) of the unwind information
    /// </summary>
    public uint UnwindInfoAddress { get; set; }

    /// <summary>
    /// Gets or sets the file offset of the function start
    /// </summary>
    public uint BeginAddressFileOffset { get; set; }

    /// <summary>
    /// Gets or sets the file offset of the unwind information
    /// </summary>
    public uint UnwindInfoFileOffset { get; set; }
}
