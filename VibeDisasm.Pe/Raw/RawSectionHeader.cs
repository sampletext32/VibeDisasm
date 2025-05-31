using System.Text;

namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Represents a raw section header in the PE file
/// </summary>
public class RawSectionHeader : IRawStructure
{
    /// <summary>
    /// Section name as a byte array (8 bytes)
    /// </summary>
    public byte[] NameBytes { get; set; } = new byte[8];

    /// <summary>
    /// Section name as a string
    /// </summary>
    public string Name
    {
        get
        {
            // Convert the name bytes to a string, stopping at the first null byte
            var length = 0;
            while (length < NameBytes.Length && NameBytes[length] != 0)
            {
                length++;
            }

            return Encoding.ASCII.GetString(NameBytes, 0, length);
        }
    }

    /// <summary>
    /// Virtual size
    /// </summary>
    public uint VirtualSize { get; set; }

    /// <summary>
    /// Virtual address
    /// </summary>
    public uint VirtualAddress { get; set; }

    /// <summary>
    /// Size of raw data
    /// </summary>
    public uint SizeOfRawData { get; set; }

    /// <summary>
    /// Pointer to raw data
    /// </summary>
    public uint PointerToRawData { get; set; }

    /// <summary>
    /// Pointer to relocations
    /// </summary>
    public uint PointerToRelocations { get; set; }

    /// <summary>
    /// Pointer to line numbers
    /// </summary>
    public uint PointerToLinenumbers { get; set; }

    /// <summary>
    /// Number of relocations
    /// </summary>
    public ushort NumberOfRelocations { get; set; }

    /// <summary>
    /// Number of line numbers
    /// </summary>
    public ushort NumberOfLinenumbers { get; set; }

    /// <summary>
    /// Characteristics
    /// </summary>
    public uint Characteristics { get; set; }

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 40; // Section header is always 40 bytes
}
