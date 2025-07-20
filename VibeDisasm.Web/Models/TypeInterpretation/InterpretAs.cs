namespace VibeDisasm.Web.Models.TypeInterpretation;

public enum InterpretAs
{
    /// <summary> Interpret bytes as-is, without any conversion </summary>
    Bytes,
    /// <summary> Interpret bytes as a big-endian signed integer </summary>
    SignedIntegerBE,
    /// <summary> Interpret bytes as a little-endian signed integer </summary>
    SignedIntegerLE,
    /// <summary> Interpret bytes as a big-endian unsigned integer </summary>
    UnsignedIntegerBE,
    /// <summary> Interpret bytes as a little-endian unsigned integer </summary>
    UnsignedIntegerLE,
    /// <summary> Interpret bytes as a floating-point number </summary>
    FloatingPoint,
    /// <summary> Interpret bytes as a boolean value </summary>
    Boolean,
    /// <summary> Interpret bytes as an ASCII string </summary>
    AsciiString,
    /// <summary> Interpret bytes as a wide (UTF-16) string </summary>
    WideString,
    /// <summary> Interpret bytes as a timestamp </summary>
    Timestamp
}
