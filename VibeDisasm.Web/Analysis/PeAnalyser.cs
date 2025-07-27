using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Overlay;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Analysis;

public class PeAnalyser(
    ITypeArchiveStorage typeArchiveStorage,
    ILogger<PeAnalyser> logger) : IAnalyser
{
    public async Task<Result> Run(RuntimeUserProgram program, byte[] binaryData, CancellationToken cancellationToken)
    {
        await Task.Yield();

        var overlayedImageDosHeader = OverlayHelper.OverlayStructure(
            program,
            typeArchiveStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_DOS_HEADER"),
            binaryData,
            0
        );

        var interpretedImageDosHeader = TypeInterpreter.Interpret(overlayedImageDosHeader);

        var e_lfanewField = overlayedImageDosHeader["e_lfanew"];

        var e_lfanew = TypeInterpreter.InterpretStructureField(e_lfanewField).
            Reinterpret<InterpretedUnsignedInteger>().
            Get();

        var overlayedImageFileHeader = OverlayHelper.OverlayStructure(
            program,
            typeArchiveStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_FILE_HEADER"),
            binaryData,
            (int)e_lfanew + 4
        );

        var timeDateStampField = overlayedImageFileHeader["TimeDateStamp"];

        var timeDateStamp = new DateTime(1970, 1, 1).AddSeconds(
            TypeInterpreter.InterpretStructureField(timeDateStampField).Reinterpret<InterpretedUnsignedInteger>().Get()
        );

        var overlayOptionalMagic = OverlayHelper.OverlayPrimitive(
            program,
            typeArchiveStorage.FindRequiredType<RuntimePrimitiveType>("win32", "WORD"),
            binaryData,
            (int)e_lfanew + 4 + overlayedImageFileHeader.Bytes.Length
        );

        var optionalMagic = TypeInterpreter.InterpretPrimitive(overlayOptionalMagic).
            Reinterpret<InterpretedUnsignedInteger>().
            Get();

        ProgramArchitecture? architecture = optionalMagic switch
        {
            0x10b => ProgramArchitecture.X86,
            0x20b => ProgramArchitecture.X64,
            _ => null
        };

        if (architecture is null)
        {
            logger.LogError("Failed to determine architecture for program {ProgramId}", program.Id);
            return Result.Fail("Failed to determine architecture");
        }

        program.Architecture = architecture.Value;

        var imageOptionalHeaderType = architecture switch
        {
            ProgramArchitecture.X86 => "IMAGE_OPTIONAL_HEADER32",
            ProgramArchitecture.X64 => "IMAGE_OPTIONAL_HEADER64",
            _ => throw new NotSupportedException($"Unsupported architecture: {architecture}")
        };

        var overlayImageOptionalHeader = OverlayHelper.OverlayStructure(
            program,
            typeArchiveStorage.FindRequiredType<RuntimeStructureType>("win32", imageOptionalHeaderType),
            binaryData,
            (int)e_lfanew + 4 + overlayedImageFileHeader.Bytes.Length
        );

        program.Database.EntryManager.AddEntry(
            new StructUserProgramDatabaseEntry(
                0,
                overlayedImageDosHeader.Bytes.Length,
                overlayedImageDosHeader.SourceStructure
            )
        );

        program.Database.EntryManager.AddEntry(
            new StructUserProgramDatabaseEntry(
                (uint)e_lfanew + 4,
                overlayedImageFileHeader.Bytes.Length,
                overlayedImageFileHeader.SourceStructure
            )
        );

        program.Database.EntryManager.AddEntry(
            new StructUserProgramDatabaseEntry(
                (uint)e_lfanew + 4 + (uint)overlayedImageFileHeader.Bytes.Length,
                overlayImageOptionalHeader.Bytes.Length,
                overlayImageOptionalHeader.SourceStructure
            )
        );

        return Result.Ok();
    }
}
