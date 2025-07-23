using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Overlay;

namespace VibeDisasm.Web.Models.TypeInterpretation;

public static class TypeInterpreter
{
    public static InterpretedValue Interpret(OverlayedType type)
    {
        return type switch
        {
            OverlayedStructure structure => InterpretStructure(structure),
            OverlayedStructureField structureField => InterpretStructureField(structureField),
            OverlayedPrimitive primitive => InterpretPrimitive(primitive),
            OverlayedArray array => InterpretArray(array),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unknown overlayed type: {type.GetType()}")
        };
    }

    public static InterpretedStructField InterpretStructureField(OverlayedStructureField field) =>
        new InterpretedStructField(field.Bytes, field.SourceField.Name, Interpret(field.OverlayedValue));
    public static InterpretedValue InterpretPrimitive(OverlayedPrimitive primitive) =>
        InterpretType(primitive.Primitive, primitive.Bytes);

    public static InterpretedValue InterpretArray(OverlayedArray array)
    {
        var interpretedValues = array.Elements.Select(Interpret).ToList();
        return new InterpretedArrayValue(array.Bytes, interpretedValues);
    }

    public static InterpretedValue InterpretStructure(OverlayedStructure structure)
    {
        var interpretedValues = structure.Fields.Select(InterpretStructureField).ToList();
        return new InterpretedStructValue(structure.Bytes, structure.SourceStructure.Name, interpretedValues);
    }

    [Pure]
    public static InterpretedValue InterpretType(
        RuntimeDatabaseType resolvedType,
        Memory<byte> bytes
    )
    {
        return resolvedType.InterpretAs switch
        {
            InterpretAs.Bytes => new InterpretedRawValue(bytes, Endianness.LittleEndian),
            InterpretAs.SignedIntegerBE => new InterpretedSignedInteger(bytes, Endianness.BigEndian),
            InterpretAs.SignedIntegerLE => new InterpretedSignedInteger(bytes, Endianness.LittleEndian),
            InterpretAs.UnsignedIntegerBE => new InterpretedUnsignedInteger(bytes, Endianness.BigEndian),
            InterpretAs.UnsignedIntegerLE => new InterpretedUnsignedInteger(bytes, Endianness.LittleEndian),
            InterpretAs.FloatingPoint => bytes.Length == 4
                ? new InterpretedFloat(bytes, Endianness.LittleEndian)
                : new InterpretedDouble(bytes, Endianness.LittleEndian),
            InterpretAs.Boolean => new InterpretedBoolean(bytes, Endianness.LittleEndian),
            InterpretAs.AsciiString => new InterpretedAsciiString(bytes),
            InterpretAs.WideString => new InterpretedWideString(bytes),
            _ => throw new ArgumentOutOfRangeException(
                nameof(resolvedType.InterpretAs),
                $"Unknown interpretation type: {resolvedType.InterpretAs}"
            )
        };
    }
}
