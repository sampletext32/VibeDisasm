using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Overlay;

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

        var overlayedImageDosHeaderResult = OverlayHelper.OverlayStructure(
            program,
            program.Database.TypeStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_DOS_HEADER"),
            binaryData,
            0
        );

        if (overlayedImageDosHeaderResult.IsFailed)
        {
            return overlayedImageDosHeaderResult.ToResult();
        }

        var overlayedImageDosHeader = overlayedImageDosHeaderResult.Value;

        var e_lfanew = overlayedImageDosHeader["e_lfanew"].
            InterpretedValue.Reinterpret<InterpretedUnsignedInteger>().
            Get();

        var overlayedImageFileHeaderResult = OverlayHelper.OverlayStructure(
            program,
            program.Database.TypeStorage.FindRequiredType<RuntimeStructureType>("win32", "IMAGE_FILE_HEADER"),
            binaryData,
            (int)e_lfanew + 4
        );

        if (overlayedImageFileHeaderResult.IsFailed)
        {
            return overlayedImageFileHeaderResult.ToResult();
        }

        var overlayedImageFileHeader = overlayedImageFileHeaderResult.Value;

        var timeDateStamp = new DateTime(1970, 1, 1).AddSeconds(
            overlayedImageFileHeader["TimeDateStamp"].InterpretedValue.Reinterpret<InterpretedUnsignedInteger>().Get()
        );

        var overlayOptionalMagicResult = OverlayHelper.OverlayPrimitive(
            program,
            program.Database.TypeStorage.FindRequiredType<RuntimePrimitiveType>("win32", "WORD"),
            binaryData,
            (int)e_lfanew + 4 + overlayedImageFileHeader.ByteSize
        );

        if (overlayOptionalMagicResult.IsFailed)
        {
            return overlayOptionalMagicResult.ToResult();
        }

        var optionalMagic = overlayOptionalMagicResult.Value.InterpretedValue.
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
            _logger.LogError("Failed to determine architecture for program {ProgramId}", program.Id);
            return Result.Fail("Failed to determine architecture");
        }

        program.Architecture = architecture.Value;

        return Result.Ok();
    }
}
