using FluentResults;
using VibeDisasm.Web.Services;
using Microsoft.Extensions.Logging;

namespace VibeDisasm.Web.Handlers;

public class ListRecentsHandler
{
    private readonly RecentsService _recents;
    private readonly ILogger<ListRecentsHandler> _logger;

    public ListRecentsHandler(RecentsService recents, ILogger<ListRecentsHandler> logger)
    {
        _recents = recents;
        _logger = logger;
    }

    [Pure]
    public Task<Result<IEnumerable<RecentJsonMetadata>>> Handle()
    {
        var result = Result.Ok(_recents.Get());
        _logger.LogInformation("Listed recent projects");
        return Task.FromResult(result);
    }
}
