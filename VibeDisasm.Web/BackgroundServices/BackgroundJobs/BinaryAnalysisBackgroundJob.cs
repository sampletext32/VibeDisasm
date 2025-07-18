using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.BackgroundServices.BackgroundJobs;

public enum BackgroundJobStatus
{
    Queued,
    Running,
    Completed,
    Failed
}

public class BinaryAnalysisBackgroundJob(Guid jobId, RuntimeUserProject project, RuntimeUserProgram program, byte[] binaryData)
    : BackgroundJob(jobId, BackgroundJobStatus.Queued)
{
    public RuntimeUserProject Project { get; init; } = project;
    public RuntimeUserProgram Program { get; init; } = program;
    public byte[] BinaryData { get; init; } = binaryData;
}
