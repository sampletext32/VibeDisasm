using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

public static class OverlayHelper
{
    public static OverlayedType OverlayType(
        IRuntimeDatabaseType type,
        Memory<byte> binaryData,
        int offset
    ) => type switch
    {
        RuntimeStructureType structure => OverlayStructure(structure, binaryData, offset),
        RuntimePrimitiveType primitive => OverlayPrimitive(primitive, binaryData, offset),
        RuntimeArrayType array => OverlayArray(array, binaryData, offset),
        _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported type {type.Name} for overlaying")
    };

    public static OverlayedStructure OverlayStructure(
        RuntimeStructureType structure,
        Memory<byte> binaryData,
        int offset
    )
    {
        var size = structure.Size;

        if (offset + size > binaryData.Length)
        {
            throw new Exception($"Overlaying structure {structure.Name} at offset {offset} exceeds binary data length");
        }

        var rawBytes = binaryData.Slice(offset, size);

        var fieldOffset = 0;
        var overlayedFields = new List<OverlayedStructureField>(structure.Fields.Count);
        for (var i = 0; i < structure.Fields.Count; i++)
        {
            var field = structure.Fields[i];
            var fieldSize = field.Type.Size;
            if (fieldSize is 0)
            {
                throw new Exception(
                    $"Field {field.Name} in structure {structure.Name} has zero size, thus the structure is corrupted"
                );
            }

            var fieldBytes = binaryData.Slice(offset + fieldOffset, fieldSize);

            var overlayedType = OverlayType(field.Type, fieldBytes, 0);

            var overlayedField = new OverlayedStructureField(
                field,
                overlayedType,
                fieldBytes
            );
            overlayedFields.Add(overlayedField);

            fieldOffset += fieldSize;
        }

        var overlayedStructure = new OverlayedStructure(structure, overlayedFields, rawBytes);
        return overlayedStructure;
    }

    public static OverlayedPrimitive OverlayPrimitive(
        RuntimePrimitiveType primitive,
        Memory<byte> binaryData,
        int offset
    )
    {
        var size = primitive.Size;

        if (offset + size > binaryData.Length)
        {
            throw new Exception($"Overlaying primitive {primitive.Name} at offset {offset} exceeds binary data length");
        }

        if (size is 0)
        {
            throw new Exception($"Primitive {primitive.Name} has zero size, thus is not supposed to be overlayed");
        }

        var rawBytes = binaryData.Slice(offset, size);

        var overlayed = new OverlayedPrimitive(
            primitive,
            rawBytes
        );

        return overlayed;
    }

    public static OverlayedArray OverlayArray(
        RuntimeArrayType arrayType,
        Memory<byte> binaryData,
        int offset
    )
    {
        var size = arrayType.Size;

        if (offset + size > binaryData.Length)
        {
            throw new Exception(
                $"Overlaying array {arrayType.Name} at offset {offset} exceeds binary data length"
            );
        }

        var rawBytes = binaryData.Slice(offset, size);

        var elementSize = arrayType.ElementType.Size;

        var overlayedElements = new List<OverlayedType>(arrayType.ElementCount);
        for (var i = 0; i < arrayType.ElementCount; i++)
        {
            var overlayedElement = OverlayType(
                arrayType.ElementType,
                rawBytes.Slice(i * elementSize, elementSize),
                offset
            );

            overlayedElements.Add(overlayedElement);
        }

        var result = new OverlayedArray(arrayType, overlayedElements, rawBytes);
        return result;
    }
}
