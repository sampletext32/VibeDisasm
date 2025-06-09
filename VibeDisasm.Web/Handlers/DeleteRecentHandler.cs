using System.Diagnostics.Contracts;
using FluentResults;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class DeleteRecentHandler
{
    private readonly RecentsService _recents;

    public DeleteRecentHandler(RecentsService recents)
    {
        _recents = recents;
    }

    [Pure]
    public Task<Result> Handle(Guid recentId)
    {
        _recents.RemoveByProjectId(recentId);

        return Task.FromResult(Result.Ok());
    }
}
