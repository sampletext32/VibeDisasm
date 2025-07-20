using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.TypeInterpretation;

public static class TypeInterpreter
{
    [Pure]
    public static InterpretedValue InterpretType(RuntimeDatabaseType type, Memory<byte> bytes, int byteLength)
    {
        if (bytes.Length < byteLength)
        {
            throw new ArgumentException(
                $"Required length ({byteLength} bytes) exceeds available memory length ({bytes.Length} bytes)"
            );
        }

        var data = bytes[..byteLength];

        return type.InterpretAs switch
        {
            InterpretAs.Bytes => new InterpretedRawValue(data, Endianness.LittleEndian),

            InterpretAs.SignedIntegerBE => new InterpretedSignedInteger(data, Endianness.BigEndian),
            InterpretAs.SignedIntegerLE => new InterpretedSignedInteger(data, Endianness.LittleEndian),
            InterpretAs.UnsignedIntegerBE => new InterpretedUnsignedInteger(data, Endianness.BigEndian),
            InterpretAs.UnsignedIntegerLE => new InterpretedUnsignedInteger(data, Endianness.LittleEndian),

            InterpretAs.FloatingPoint => byteLength == 4
                ? new InterpretedFloat(data, Endianness.LittleEndian)
                : new InterpretedDouble(data, Endianness.LittleEndian),
            InterpretAs.Boolean => new InterpretedBoolean(data, Endianness.LittleEndian),

            InterpretAs.AsciiString => new InterpretedAsciiString(data),
            InterpretAs.WideString => new InterpretedWideString(data),

            _ => throw new ArgumentOutOfRangeException(
                nameof(type.InterpretAs),
                $"Unknown interpretation type: {type.InterpretAs}"
            )
        };
    }
}
