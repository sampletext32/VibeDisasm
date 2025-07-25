using System.Threading.Channels;
using VibeDisasm.Web.Analysis;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;

namespace VibeDisasm.Web.BackgroundServices;

public class BinaryAnalysisBackgroundService(
    AnalysisExecutor analysisExecutor,
    Channel<BinaryAnalysisJob> channel,
    ILogger<BinaryAnalysisBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in channel.Reader.ReadAllAsync(stoppingToken))
        {
            job.Status = BackgroundJobStatus.Running;
            logger.LogInformation(
                "Launched analysis for program {ProgramId} in project {ProjectId}",
                job.Program.Id,
                job.Project.Id
            );

            var result = await analysisExecutor.DoAnalysis(job, stoppingToken);
            job.Status = result.IsSuccess ? BackgroundJobStatus.Completed : BackgroundJobStatus.Failed;

            logger.LogInformation(
                "Finished analysis for program {ProgramId} in project {ProjectId}",
                job.Program.Id,
                job.Project.Id
            );
        }
    }
}
