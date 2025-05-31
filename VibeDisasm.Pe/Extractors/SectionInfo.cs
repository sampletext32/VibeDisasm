namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Represents extracted information about a PE file section
/// </summary>
public class SectionInfo
{
    /// <summary>
    /// The name of the section
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The virtual address of the section
    /// </summary>
    public uint VirtualAddress { get; set; }

    /// <summary>
    /// The virtual size of the section
    /// </summary>
    public uint VirtualSize { get; set; }

    /// <summary>
    /// The raw data address (file offset) of the section
    /// </summary>
    public uint RawDataAddress { get; set; }

    /// <summary>
    /// The raw data size of the section
    /// </summary>
    public uint RawDataSize { get; set; }

    /// <summary>
    /// The section characteristics
    /// </summary>
    public uint Characteristics { get; set; }

    /// <summary>
    /// The raw data of the section
    /// </summary>
    public byte[] Data { get; set; } = [];

    /// <summary>
    /// Gets whether the section is executable
    /// </summary>
    public bool IsExecutable => (Characteristics & SectionCharacteristics.Executable) != 0;

    /// <summary>
    /// Gets whether the section is readable
    /// </summary>
    public bool IsReadable => (Characteristics & SectionCharacteristics.Readable) != 0;

    /// <summary>
    /// Gets whether the section is writable
    /// </summary>
    public bool IsWritable => (Characteristics & SectionCharacteristics.Writable) != 0;

    /// <summary>
    /// Gets whether the section contains code
    /// </summary>
    public bool ContainsCode => (Characteristics & SectionCharacteristics.ContainsCode) != 0;

    /// <summary>
    /// Gets whether the section contains initialized data
    /// </summary>
    public bool ContainsInitializedData => (Characteristics & SectionCharacteristics.ContainsInitializedData) != 0;

    /// <summary>
    /// Gets whether the section contains uninitialized data
    /// </summary>
    public bool ContainsUninitializedData => (Characteristics & SectionCharacteristics.ContainsUninitializedData) != 0;
}
