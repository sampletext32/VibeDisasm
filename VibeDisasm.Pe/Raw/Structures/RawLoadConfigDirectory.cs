namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw load config directory in the PE file (IMAGE_LOAD_CONFIG_DIRECTORY)
/// </summary>
public class RawLoadConfigDirectory : IRawStructure
{
    /// <summary>
    /// Size of the structure
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// Time/date stamp
    /// </summary>
    public uint TimeDateStamp { get; set; }

    /// <summary>
    /// Major version
    /// </summary>
    public ushort MajorVersion { get; set; }

    /// <summary>
    /// Minor version
    /// </summary>
    public ushort MinorVersion { get; set; }

    /// <summary>
    /// Global flags to clear
    /// </summary>
    public uint GlobalFlagsClear { get; set; }

    /// <summary>
    /// Global flags to set
    /// </summary>
    public uint GlobalFlagsSet { get; set; }

    /// <summary>
    /// Critical section default timeout
    /// </summary>
    public uint CriticalSectionDefaultTimeout { get; set; }

    /// <summary>
    /// De-commit free block threshold
    /// </summary>
    public ulong DeCommitFreeBlockThreshold { get; set; }

    /// <summary>
    /// De-commit total free threshold
    /// </summary>
    public ulong DeCommitTotalFreeThreshold { get; set; }

    /// <summary>
    /// Lock prefix table
    /// </summary>
    public ulong LockPrefixTable { get; set; }

    /// <summary>
    /// Maximum allocation size
    /// </summary>
    public ulong MaximumAllocationSize { get; set; }

    /// <summary>
    /// Virtual memory threshold
    /// </summary>
    public ulong VirtualMemoryThreshold { get; set; }

    /// <summary>
    /// Process heap flags
    /// </summary>
    public ulong ProcessAffinityMask { get; set; }

    /// <summary>
    /// Process heap flags
    /// </summary>
    public uint ProcessHeapFlags { get; set; }

    /// <summary>
    /// CSD version
    /// </summary>
    public ushort CSDVersion { get; set; }

    /// <summary>
    /// Reserved
    /// </summary>
    public ushort Reserved1 { get; set; }

    /// <summary>
    /// Edit list
    /// </summary>
    public ulong EditList { get; set; }

    /// <summary>
    /// Security cookie
    /// </summary>
    public ulong SecurityCookie { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes (variable, depends on version)
    /// </summary>
    int IRawStructure.Size => (int)Size; // Size is variable and specified in the structure itself
}
