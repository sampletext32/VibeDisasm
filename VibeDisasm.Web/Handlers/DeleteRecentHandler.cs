using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class DeleteRecentHandler(
    RecentsService recents,
    ILogger<DeleteRecentHandler> logger
) : IHandler
{
    public Task<Result> Handle(Guid recentId)
    {
        recents.RemoveByProjectId(recentId);
        logger.LogInformation("Deleted recent project {RecentId}", recentId);
        return Task.FromResult(Result.Ok());
    }
}
