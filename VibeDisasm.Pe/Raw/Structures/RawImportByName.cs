namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw import by name structure in the PE file (IMAGE_IMPORT_BY_NAME)
/// </summary>
public class RawImportByName : IRawStructure
{
    /// <summary>
    /// Hint value (index into export name pointer table)
    /// </summary>
    public ushort Hint { get; set; }

    /// <summary>
    /// The imported function name as a byte array (null-terminated ASCII string)
    /// </summary>
    public byte[] NameBytes { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets the size of the structure in bytes (variable length)
    /// </summary>
    public int Size => 2 + NameBytes.Length; // 2 bytes for Hint + variable length name
}
