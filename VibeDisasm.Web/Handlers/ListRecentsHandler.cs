using FluentResults;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class ListRecentsHandler
{
    private readonly RecentsService _recents;

    public ListRecentsHandler(RecentsService recents)
    {
        _recents = recents;
    }

    [Pure]
    public Task<Result<IEnumerable<RecentJsonMetadata>>> Handle()
    {
        var result = Result.Ok(_recents.Get());
        return Task.FromResult(result);
    }
}
