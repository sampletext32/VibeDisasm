using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive;

namespace VibeDisasm.Web.Overlay;

public static class OverlayHelper
{
    public static Result<OverlayedType> OverlayType(
        RuntimeUserProgram program,
        RuntimeDatabaseType type,
        Memory<byte> binaryData,
        int offset
    )
    {
        return type switch
        {
            RuntimeStructureType structure => OverlayStructure(program, structure, binaryData, offset),
            RuntimePrimitiveType primitive => OverlayPrimitive(program, primitive, binaryData, offset),
            RuntimeArrayType array => OverlayArray(program, array, binaryData, offset),
            _ => Result.Fail($"Unsupported type {type.Name} for overlaying in program {program.Id}")
        };
    }

    public static OverlayedStructure OverlayStructure(
        RuntimeUserProgram program,
        RuntimeStructureType structure,
        Memory<byte> binaryData,
        int offset
    )
    {
        var typeSizeVisitor = new TypeSizeVisitor(program);
        var size = typeSizeVisitor.Visit(structure);

        if (offset + size > binaryData.Length)
        {
            throw new Exception($"Overlaying structure {structure.Name} at offset {offset} exceeds binary data length for program {program.Id}");
        }

        var rawBytes = binaryData.Slice(offset, size);

        var fieldOffset = 0;
        var overlayedFields = new List<OverlayedStructureField>(structure.Fields.Count);
        for (var i = 0; i < structure.Fields.Count; i++)
        {
            var field = structure.Fields[i];
            var fieldSize = typeSizeVisitor.Visit(field.Type);
            if (fieldSize is 0)
            {
                throw new Exception($"Field {field.Name} in structure {structure.Name} has zero size, thus the structure is corrupted");
            }

            var resolvedFieldType = program.Database.TypeStorage.DeepResolveTypeRef(field.Type);

            if (resolvedFieldType is null)
            {
                throw new Exception($"Could not resolve type for field {field.Name} in structure {structure.Name}");
            }

            var fieldBytes = binaryData.Slice(offset + fieldOffset, fieldSize);

            var overlayFieldResult = OverlayType(program, resolvedFieldType, fieldBytes, 0);

            if (overlayFieldResult.IsFailed)
            {
                throw new Exception(overlayFieldResult.Errors.First().Message);
            }

            var overlayedField = new OverlayedStructureField(
                field,
                overlayFieldResult.Value,
                fieldBytes
            );
            overlayedFields.Add(overlayedField);

            fieldOffset += fieldSize;
        }

        var overlayedStructure = new OverlayedStructure(structure, overlayedFields, rawBytes);
        return overlayedStructure;
    }

    public static OverlayedPrimitive OverlayPrimitive(
        RuntimeUserProgram program,
        RuntimePrimitiveType primitive,
        Memory<byte> binaryData,
        int offset
    )
    {
        var typeSizeVisitor = new TypeSizeVisitor(program);
        var size = typeSizeVisitor.Visit(primitive);

        if (offset + size > binaryData.Length)
        {
            throw new Exception($"Overlaying primitive {primitive.Name} at offset {offset} exceeds binary data length for program {program.Id}");
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
        RuntimeUserProgram program,
        RuntimeArrayType arrayType,
        Memory<byte> binaryData,
        int offset
    )
    {
        var typeSizeVisitor = new TypeSizeVisitor(program);
        var size = typeSizeVisitor.Visit(arrayType);

        if (offset + size > binaryData.Length)
        {
            throw new Exception($"Overlaying array {arrayType.Name} at offset {offset} exceeds binary data length for program {program.Id}");
        }

        var rawBytes = binaryData.Slice(offset, size);
        var elementType = program.Database.TypeStorage.DeepResolveTypeRef(arrayType.ElementType);

        if (elementType is null)
        {
            throw new Exception($"Could not resolve element type for array {arrayType.Name}");
        }

        var elementSize = typeSizeVisitor.Visit(elementType);

        var overlayedElements = new List<OverlayedType>(arrayType.ElementCount);
        for (var i = 0; i < arrayType.ElementCount; i++)
        {
            var overlayElementResult = OverlayType(program, elementType, rawBytes.Slice(i * elementSize, elementSize), offset);

            if (overlayElementResult.IsFailed)
            {
                throw new Exception(overlayElementResult.Errors.First().Message);
            }

            overlayedElements.Add(overlayElementResult.Value);
        }
        var result = new OverlayedArray(arrayType, elementType, overlayedElements, rawBytes);
        return result;
    }
}
