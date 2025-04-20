namespace VibeDisasm.Pe.Raw.Structures;

/// <summary>
/// Represents a raw import thunk in the PE file (IMAGE_THUNK_DATA)
/// </summary>
public class RawImportThunk : IRawStructure
{
    /// <summary>
    /// Union of ForwarderString, Function, Ordinal, AddressOfData
    /// </summary>
    public ulong Value { get; set; }

    /// <summary>
    /// Gets whether this thunk uses an ordinal
    /// </summary>
    public bool IsOrdinal => (Value & 0x8000000000000000) != 0;

    /// <summary>
    /// Gets the ordinal value if this thunk uses an ordinal
    /// </summary>
    public ushort Ordinal => (ushort)(Value & 0xFFFF);

    /// <summary>
    /// Gets the hint/name table RVA if this thunk does not use an ordinal
    /// </summary>
    public uint HintNameTableRva => (uint)(Value & 0x7FFFFFFF);

    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    public int Size => 8; // Import thunk is 4 bytes in PE32, 8 bytes in PE32+
}
