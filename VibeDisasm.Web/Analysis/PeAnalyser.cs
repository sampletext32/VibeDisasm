using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Overlay;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Analysis;

public class PeAnalyser(
    ITypeArchiveStorage typeArchiveStorage,
    ILogger<PeAnalyser> logger
) : IAnalyser
{
    public async Task<Result> Run(RuntimeUserProgram program, byte[] binaryData, CancellationToken cancellationToken)
    {
        await Task.Yield();

        // since we know this is a PE file, import required types

        var builtinArchive = typeArchiveStorage.FindRequiredArchive("builtin");
        var win32Archive = typeArchiveStorage.FindRequiredArchive("win32");

        program.ReferencedTypeArchives.Add(builtinArchive);
        program.ReferencedTypeArchives.Add(win32Archive);

        var overlayedImageDosHeader = OverlayHelper.OverlayStructure(
            typeArchiveStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_DOS_HEADER"),
            binaryData,
            0
        );

        var e_lfanewField = overlayedImageDosHeader["e_lfanew"];

        var e_lfanew = (int)TypeInterpreter.Interpret2<InterpretValue2U4>(e_lfanewField).Value;

        var overlayedImageFileHeader = OverlayHelper.OverlayStructure(
            typeArchiveStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_FILE_HEADER"),
            binaryData,
            (int)e_lfanew + 4
        );

        var overlayOptionalMagic = OverlayHelper.OverlayPrimitive(
            typeArchiveStorage.FindRequiredType<RuntimePrimitiveType>("win32", "WORD"),
            binaryData,
            (int)e_lfanew + 4 + overlayedImageFileHeader.Bytes.Length
        );

        var optionalMagic = TypeInterpreter.Interpret2<InterpretValue2U2>(overlayOptionalMagic).Value;

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

        var numberOfSectionsField = overlayedImageFileHeader["NumberOfSections"];

        var sectionsStart = e_lfanew + 4 + overlayedImageFileHeader.Bytes.Length +
                            overlayImageOptionalHeader.Bytes.Length;

        var numberOfSections = TypeInterpreter.Interpret2<InterpretValue2U2>(numberOfSectionsField).Value;

        var imageSectionHeaderType = typeArchiveStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_SECTION_HEADER");

        var imageSectionHeaderSize = imageSectionHeaderType.Size;
        for (var i = 0; i < numberOfSections; i++)
        {
            var overlaySection = OverlayHelper.OverlayStructure(
                imageSectionHeaderType,
                binaryData,
                sectionsStart + i * imageSectionHeaderSize
            );

            var section = TypeInterpreter.Interpret2<InterpretValue2Struct>(overlaySection);

            program.Database.EntryManager.AddEntry(
                new StructUserProgramDatabaseEntry(
                    (uint)sectionsStart + (uint)(i * imageSectionHeaderSize),
                    overlaySection.Bytes.Length,
                    overlaySection.SourceStructure
                )
            );
        }

        return Result.Ok();
    }
}
