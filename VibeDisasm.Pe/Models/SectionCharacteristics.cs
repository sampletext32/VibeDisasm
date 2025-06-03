namespace VibeDisasm.Pe.Models;

/// <summary>
/// Constants for PE section characteristics
/// </summary>
public static class SectionCharacteristics
{
    /// <summary>
    /// The section contains executable code
    /// </summary>
    public const uint ContainsCode = 0x00000020;

    /// <summary>
    /// The section contains initialized data
    /// </summary>
    public const uint ContainsInitializedData = 0x00000040;

    /// <summary>
    /// The section contains uninitialized data
    /// </summary>
    public const uint ContainsUninitializedData = 0x00000080;

    /// <summary>
    /// The section contains comments or other information
    /// </summary>
    public const uint ContainsInfo = 0x00000200;

    /// <summary>
    /// The section will not become part of the image
    /// </summary>
    public const uint RemoveFromImage = 0x00000800;

    /// <summary>
    /// The section contains COMDAT data
    /// </summary>
    public const uint ContainsComdat = 0x00001000;

    /// <summary>
    /// The section contains data referenced through the global pointer
    /// </summary>
    public const uint GpRelative = 0x00008000;

    /// <summary>
    /// Align data on a 1-byte boundary
    /// </summary>
    public const uint Align1Bytes = 0x00100000;

    /// <summary>
    /// Align data on a 2-byte boundary
    /// </summary>
    public const uint Align2Bytes = 0x00200000;

    /// <summary>
    /// Align data on a 4-byte boundary
    /// </summary>
    public const uint Align4Bytes = 0x00300000;

    /// <summary>
    /// Align data on a 8-byte boundary
    /// </summary>
    public const uint Align8Bytes = 0x00400000;

    /// <summary>
    /// Align data on a 16-byte boundary
    /// </summary>
    public const uint Align16Bytes = 0x00500000;

    /// <summary>
    /// Align data on a 32-byte boundary
    /// </summary>
    public const uint Align32Bytes = 0x00600000;

    /// <summary>
    /// Align data on a 64-byte boundary
    /// </summary>
    public const uint Align64Bytes = 0x00700000;

    /// <summary>
    /// Align data on a 128-byte boundary
    /// </summary>
    public const uint Align128Bytes = 0x00800000;

    /// <summary>
    /// Align data on a 256-byte boundary
    /// </summary>
    public const uint Align256Bytes = 0x00900000;

    /// <summary>
    /// Align data on a 512-byte boundary
    /// </summary>
    public const uint Align512Bytes = 0x00A00000;

    /// <summary>
    /// Align data on a 1024-byte boundary
    /// </summary>
    public const uint Align1024Bytes = 0x00B00000;

    /// <summary>
    /// Align data on a 2048-byte boundary
    /// </summary>
    public const uint Align2048Bytes = 0x00C00000;

    /// <summary>
    /// Align data on a 4096-byte boundary
    /// </summary>
    public const uint Align4096Bytes = 0x00D00000;

    /// <summary>
    /// Align data on a 8192-byte boundary
    /// </summary>
    public const uint Align8192Bytes = 0x00E00000;

    /// <summary>
    /// The section contains extended relocations
    /// </summary>
    public const uint ContainsExtendedRelocations = 0x01000000;

    /// <summary>
    /// The section can be discarded as needed
    /// </summary>
    public const uint Discardable = 0x02000000;

    /// <summary>
    /// The section cannot be cached
    /// </summary>
    public const uint NotCacheable = 0x04000000;

    /// <summary>
    /// The section cannot be paged
    /// </summary>
    public const uint NotPageable = 0x08000000;

    /// <summary>
    /// The section can be shared in memory
    /// </summary>
    public const uint Shared = 0x10000000;

    /// <summary>
    /// The section can be executed as code
    /// </summary>
    public const uint Executable = 0x20000000;

    /// <summary>
    /// The section can be read
    /// </summary>
    public const uint Readable = 0x40000000;

    /// <summary>
    /// The section can be written to
    /// </summary>
    public const uint Writable = 0x80000000;

    /// <summary>
    /// Mask to extract alignment values
    /// </summary>
    public const uint AlignmentMask = 0x00F00000;
}
