namespace VibeDisasm.Web.Models.TypeInterpretation;

public enum Endianness
{
    /// <summary>
    /// Big-endian format, where the most significant byte is stored at the lowest memory address.
    /// </summary>
    BigEndian,

    /// <summary>
    /// Little-endian format, where the least significant byte is stored at the lowest memory address.
    /// </summary>
    LittleEndian
}
