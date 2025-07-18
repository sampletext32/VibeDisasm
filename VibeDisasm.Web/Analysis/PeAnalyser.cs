using FluentResults;
using VibeDisasm.Web.Models;

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

        var architecture = PeBitnessAnalyser.Run(binaryData);

        if (architecture is null)
        {
            _logger.LogError("Failed to determine architecture for program {ProgramId}", program.Id);
            return Result.Fail("Failed to determine architecture");
        }

        program.Architecture = architecture.Value;

        return Result.Ok();
    }
}
