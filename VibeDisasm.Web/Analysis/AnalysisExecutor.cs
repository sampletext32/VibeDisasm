using FluentResults;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;

namespace VibeDisasm.Web.Analysis;

public class AnalysisExecutor(
    AnalyserResolver analyserResolver,
    ILogger<AnalysisExecutor> logger
)
{
    public async Task<Result> DoAnalysis(BinaryAnalysisJob job, CancellationToken cancellationToken)
    {
        var programKind = DetermineProgramKindAnalyser.Run(job.BinaryData);

        if (programKind is null)
        {
            logger.LogWarning(
                "Failed to determine program kind for program {ProgramId} in project {ProjectId}",
                job.Program.Id,
                job.Project.Id
            );
            return Result.Fail("Failed to determine program kind");
        }

        job.Program.Kind = programKind.Value;

        var analyser = analyserResolver.Resolve(programKind.Value);

        if (analyser is not null)
        {
            var result = await analyser.Run(job.Program, job.BinaryData, cancellationToken);
            return result;
        }
        else
        {
            logger.LogError(
                "No analyser found for program kind {ProgramKind} for program {ProgramId} in project {ProjectId}",
                programKind.Value,
                job.Program.Id,
                job.Project.Id
            );
            return Result.Fail($"No analyser found for program kind {programKind.Value}");
        }
    }
}
