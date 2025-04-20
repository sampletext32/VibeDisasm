namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Represents the raw optional header of a PE file
/// </summary>
public class RawOptionalHeader : IRawStructure
{
    /// <summary>
    /// Magic number (0x10B for PE32, 0x20B for PE32+)
    /// </summary>
    public ushort Magic { get; set; }

    /// <summary>
    /// Linker major version
    /// </summary>
    public byte MajorLinkerVersion { get; set; }

    /// <summary>
    /// Linker minor version
    /// </summary>
    public byte MinorLinkerVersion { get; set; }

    /// <summary>
    /// Size of code section
    /// </summary>
    public uint SizeOfCode { get; set; }

    /// <summary>
    /// Size of initialized data
    /// </summary>
    public uint SizeOfInitializedData { get; set; }

    /// <summary>
    /// Size of uninitialized data
    /// </summary>
    public uint SizeOfUninitializedData { get; set; }

    /// <summary>
    /// Address of entry point
    /// </summary>
    public uint AddressOfEntryPoint { get; set; }

    /// <summary>
    /// Base of code
    /// </summary>
    public uint BaseOfCode { get; set; }

    /// <summary>
    /// Base of data (PE32 only)
    /// </summary>
    public uint BaseOfData { get; set; } // Only in PE32, not in PE32+

    /// <summary>
    /// Image base (32-bit in PE32, 64-bit in PE32+)
    /// </summary>
    public ulong ImageBase { get; set; }

    /// <summary>
    /// Section alignment
    /// </summary>
    public uint SectionAlignment { get; set; }

    /// <summary>
    /// File alignment
    /// </summary>
    public uint FileAlignment { get; set; }

    /// <summary>
    /// Major operating system version
    /// </summary>
    public ushort MajorOperatingSystemVersion { get; set; }

    /// <summary>
    /// Minor operating system version
    /// </summary>
    public ushort MinorOperatingSystemVersion { get; set; }

    /// <summary>
    /// Major image version
    /// </summary>
    public ushort MajorImageVersion { get; set; }

    /// <summary>
    /// Minor image version
    /// </summary>
    public ushort MinorImageVersion { get; set; }

    /// <summary>
    /// Major subsystem version
    /// </summary>
    public ushort MajorSubsystemVersion { get; set; }

    /// <summary>
    /// Minor subsystem version
    /// </summary>
    public ushort MinorSubsystemVersion { get; set; }

    /// <summary>
    /// Win32 version value
    /// </summary>
    public uint Win32VersionValue { get; set; }

    /// <summary>
    /// Size of image
    /// </summary>
    public uint SizeOfImage { get; set; }

    /// <summary>
    /// Size of headers
    /// </summary>
    public uint SizeOfHeaders { get; set; }

    /// <summary>
    /// Checksum
    /// </summary>
    public uint CheckSum { get; set; }

    /// <summary>
    /// Subsystem
    /// </summary>
    public ushort Subsystem { get; set; }

    /// <summary>
    /// DLL characteristics
    /// </summary>
    public ushort DllCharacteristics { get; set; }

    /// <summary>
    /// Size of stack reserve (32-bit in PE32, 64-bit in PE32+)
    /// </summary>
    public ulong SizeOfStackReserve { get; set; }

    /// <summary>
    /// Size of stack commit (32-bit in PE32, 64-bit in PE32+)
    /// </summary>
    public ulong SizeOfStackCommit { get; set; }

    /// <summary>
    /// Size of heap reserve (32-bit in PE32, 64-bit in PE32+)
    /// </summary>
    public ulong SizeOfHeapReserve { get; set; }

    /// <summary>
    /// Size of heap commit (32-bit in PE32, 64-bit in PE32+)
    /// </summary>
    public ulong SizeOfHeapCommit { get; set; }

    /// <summary>
    /// Loader flags
    /// </summary>
    public uint LoaderFlags { get; set; }

    /// <summary>
    /// Number of RVA and sizes (data directories)
    /// </summary>
    public uint NumberOfRvaAndSizes { get; set; }

    /// <summary>
    /// Data directories
    /// </summary>
    public RawDataDirectory[] DataDirectories { get; set; } = new RawDataDirectory[16];

    /// <summary>
    /// Gets whether this is a 64-bit PE file (PE32+)
    /// </summary>
    public bool IsPe32Plus => Magic == 0x20B;

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => IsPe32Plus ? 240 : 224; // Size depends on whether it's PE32 or PE32+
}
