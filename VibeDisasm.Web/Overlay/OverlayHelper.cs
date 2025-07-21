using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive;

namespace VibeDisasm.Web.Overlay;

public static class OverlayHelper
{
    public static Result<OverlayedStructure> OverlayStructure(
        RuntimeUserProgram program,
        RuntimeStructureType structure,
        byte[] binaryData,
        int offset
    )
    {
        var typeSizeVisitor = new TypeSizeVisitor(program);
        var size = typeSizeVisitor.Visit(structure);

        if (offset + size > binaryData.Length)
        {
            return Result.Fail($"Overlaying structure {structure.Name} at offset {offset} exceeds binary data length for program {program.Id}");
        }

        var overlayedFields = new List<OverlayedStructureField>(structure.Fields.Count);

        var fieldOffset = 0;
        for (var i = 0; i < structure.Fields.Count; i++)
        {
            var field = structure.Fields[i];
            var fieldSize = typeSizeVisitor.Visit(field.Type);
            if (fieldSize is 0)
            {
                return Result.Fail($"Field {field.Name} in structure {structure.Name} has zero size, thus the structure is corrupted");
            }

            var rawBytes = new Memory<byte>(binaryData, offset + fieldOffset, fieldSize);

            // Interpret the field value based on its type
            InterpretedValue interpretedValue;
            try
            {
                var databaseType = program.Database.TypeStorage.DeepResolveTypeRef(field.Type);
                if (databaseType != null)
                {
                    interpretedValue = TypeInterpreter.InterpretType(databaseType, rawBytes, fieldSize);
                }
                else
                {
                    interpretedValue = new InterpretedRawValue(rawBytes, Endianness.LittleEndian);
                }
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(
                    $"Failed to interpret field {field.Name} in structure {structure.Name}: {ex.Message}"
                );
            }

            var overlayedField = new OverlayedStructureField(
                field,
                interpretedValue
            );
            overlayedFields.Add(overlayedField);

            fieldOffset += fieldSize;
        }

        return Result.Ok(new OverlayedStructure(structure, overlayedFields, fieldOffset));
    }

    public static Result<OverlayedPrimitive> OverlayPrimitive(
        RuntimeUserProgram program,
        RuntimePrimitiveType primitive,
        byte[] binaryData,
        int offset
    )
    {
        var typeSizeVisitor = new TypeSizeVisitor(program);
        var size = typeSizeVisitor.Visit(primitive);

        if (offset + size > binaryData.Length)
        {
            return Result.Fail($"Overlaying primitive {primitive.Name} at offset {offset} exceeds binary data length for program {program.Id}");
        }

        if (size is 0)
        {
            return Result.Fail($"Primitive {primitive.Name} has zero size, thus is not supposed to be overlayed");
        }

        var rawBytes = new Memory<byte>(binaryData, offset, size);

        // Interpret the field value based on its type
        InterpretedValue interpretedValue;
        try
        {
            interpretedValue = TypeInterpreter.InterpretType(primitive, rawBytes, size);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail(
                $"Failed to interpret primitive {primitive.Name}: {ex.Message}"
            );
        }

        var overlayed = new OverlayedPrimitive(
            primitive,
            interpretedValue
        );

        return Result.Ok(overlayed);
    }
}
