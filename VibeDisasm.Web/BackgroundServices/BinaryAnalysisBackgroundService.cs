using System.Threading.Channels;
using FluentResults;
using VibeDisasm.Web.Analysis;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;

namespace VibeDisasm.Web.BackgroundServices;

public class BinaryAnalysisBackgroundService : BackgroundService
{
    private readonly AnalyserResolver _analyserResolver;
    private readonly Channel<BinaryAnalysisBackgroundJob> _channel;
    private readonly ILogger<BinaryAnalysisBackgroundService> _logger;

    public BinaryAnalysisBackgroundService(
        AnalyserResolver analyserResolver,
        Channel<BinaryAnalysisBackgroundJob> channel,
        ILogger<BinaryAnalysisBackgroundService> logger
    )
    {
        _analyserResolver = analyserResolver;
        _channel = channel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            job.Status = BackgroundJobStatus.Running;
            _logger.LogInformation(
                "Launched analysis for program {ProgramId} in project {ProjectId}",
                job.Project.Id,
                job.Program.Id
            );

            var result = await DoAnalysis(job, stoppingToken);
            job.Status = result.IsSuccess ? BackgroundJobStatus.Completed : BackgroundJobStatus.Failed;

            _logger.LogInformation(
                "Finished analysis for program {ProgramId} in project {ProjectId}",
                job.Project.Id,
                job.Program.Id
            );
        }
    }

    private async Task<Result> DoAnalysis(BinaryAnalysisBackgroundJob job, CancellationToken cancellationToken)
    {
        var programKind = DetermineProgramKindAnalyser.Run(job.BinaryData);

        if (programKind is null)
        {
            _logger.LogWarning(
                "Failed to determine program kind for program {ProgramId} in project {ProjectId}",
                job.Program.Id,
                job.Project.Id
            );
            return Result.Fail("Failed to determine program kind");
        }

        job.Program.Kind = programKind.Value;

        var analyser = _analyserResolver.Resolve(programKind.Value);

        if (analyser is not null)
        {
            var result = await analyser.Run(job.Program, job.BinaryData, cancellationToken);
            return result;
        }
        else
        {
            _logger.LogError(
                "No analyser found for program kind {ProgramKind} for program {ProgramId} in project {ProjectId}",
                programKind.Value,
                job.Program.Id,
                job.Project.Id
            );
            return Result.Fail($"No analyser found for program kind {programKind.Value}");
        }
    }
}
