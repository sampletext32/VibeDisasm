using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class ListRecentsHandler(
    RecentsService recents,
    ILogger<ListRecentsHandler> logger
) : IHandler
{
    [Pure]
    public Task<Result<IEnumerable<RecentJsonMetadata>>> Handle()
    {
        var result = Result.Ok(recents.Get());
        logger.LogInformation("Listed recent projects");
        return Task.FromResult(result);
    }
}
