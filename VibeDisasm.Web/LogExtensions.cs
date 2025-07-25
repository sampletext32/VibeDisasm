namespace VibeDisasm.Web;

public static partial class LogExtensions
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Project {ProjectId} not found")]
    public static partial void ProjectNotFound(this ILogger logger, Guid projectId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Program {ProgramId} not found in project {ProjectId}")]
    public static partial void ProgramNotFound(this ILogger logger, Guid programId, Guid projectId);
}
