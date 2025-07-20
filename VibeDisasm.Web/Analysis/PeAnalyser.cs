using System.Diagnostics;
using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive;

namespace VibeDisasm.Web.Analysis;

public class PeAnalyser : IAnalyser
{
    private readonly ILogger<PeAnalyser> _logger;

    public PeAnalyser(ILogger<PeAnalyser> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Run(RuntimeUserProgram program, byte[] binaryData, CancellationToken cancellationToken)
    {
        await Task.Yield();

        var overlayedImageDosHeader = OverlayStructure(
            program,
            program.Database.TypeStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_DOS_HEADER"),
            binaryData,
            0
        );

        var e_lfanew = overlayedImageDosHeader["e_lfanew"].
            InterpretedValue.Reinterpret<InterpretedUnsignedInteger>().
            Get();

        var overlayedImageFileHeader = OverlayStructure(
            program,
            program.Database.TypeStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_FILE_HEADER"),
            binaryData,
            (int)e_lfanew + 4
        );

        var timeDateStamp = new DateTime(1970, 1, 1).AddSeconds(
            overlayedImageFileHeader["TimeDateStamp"].InterpretedValue.Reinterpret<InterpretedUnsignedInteger>().Get()
        );

        var architecture = PeBitnessAnalyser.Run(binaryData);

        if (architecture is null)
        {
            _logger.LogError("Failed to determine architecture for program {ProgramId}", program.Id);
            return Result.Fail("Failed to determine architecture");
        }

        program.Architecture = architecture.Value;

        return Result.Ok();
    }

    public OverlayedStructure OverlayStructure(
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
            _logger.LogWarning(
                "Overlaying structure {StructureName} at offset {Offset} exceeds binary data length for program {ProgramId}",
                structure.Name,
                offset,
                program.Id
            );
            return new OverlayedStructure(structure, []);
        }

        var overlayedFields = new List<OverlayedStructureField>(structure.Fields.Count);

        var fieldOffset = 0;
        for (var i = 0; i < structure.Fields.Count; i++)
        {
            var field = structure.Fields[i];
            var fieldSize = typeSizeVisitor.Visit(field.Type);
            if (fieldSize is 0)
            {
                throw new InvalidOperationException(
                    $"Field {field.Name} in structure {structure.Name} has zero size, thus the structure is corrupted"
                );
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
                _logger.LogWarning(
                    "Failed to interpret field {FieldName} in structure {StructureName}: {ErrorMessage}",
                    field.Name,
                    structure.Name,
                    ex.Message
                );
                interpretedValue = new InterpretedRawValue(rawBytes, Endianness.LittleEndian);
            }

            var overlayedField = new OverlayedStructureField(
                field,
                rawBytes,
                interpretedValue
            );
            overlayedFields.Add(overlayedField);

            fieldOffset += fieldSize;
        }

        return new OverlayedStructure(structure, overlayedFields);
    }
}

[DebuggerDisplay("{Field.Name} ({RawBytes.Length} bytes) = {InterpretedValue.DebugDisplay}")]
public record OverlayedStructureField(
    RuntimeStructureTypeField Field,
    Memory<byte> RawBytes,
    InterpretedValue InterpretedValue
);

[DebuggerDisplay("overlayed {Structure.Name} ({Structure.Fields.Count} fields)")]
public record OverlayedStructure(RuntimeStructureType Structure, IReadOnlyList<OverlayedStructureField> Fields)
{
    public OverlayedStructureField this[int index] => Fields[index];
    public OverlayedStructureField this[string name] => Fields.First(x => x.Field.Name == name);
};
