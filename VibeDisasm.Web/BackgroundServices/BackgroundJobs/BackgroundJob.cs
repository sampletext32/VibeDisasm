namespace VibeDisasm.Web.BackgroundServices.BackgroundJobs;

/// <summary>
/// Any background job
/// </summary>
public abstract class BackgroundJob(Guid jobId, BackgroundJobStatus status)
{
    public Guid JobId { get; init; } = jobId;
    public BackgroundJobStatus Status { get; set; } = status;
}
