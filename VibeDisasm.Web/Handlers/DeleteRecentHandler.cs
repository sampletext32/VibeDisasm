using FluentResults;
using VibeDisasm.Web.Services;
using Microsoft.Extensions.Logging;

namespace VibeDisasm.Web.Handlers;

public class DeleteRecentHandler
{
    private readonly RecentsService _recents;
    private readonly ILogger<DeleteRecentHandler> _logger;

    public DeleteRecentHandler(RecentsService recents, ILogger<DeleteRecentHandler> logger)
    {
        _recents = recents;
        _logger = logger;
    }

    public Task<Result> Handle(Guid recentId)
    {
        _recents.RemoveByProjectId(recentId);
        _logger.LogInformation("Deleted recent project {RecentId}", recentId);
        return Task.FromResult(Result.Ok());
    }
}
