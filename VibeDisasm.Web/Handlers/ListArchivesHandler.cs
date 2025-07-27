using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class ListArchivesHandler(
    ITypeArchiveStorage typeArchiveStorage,
    ILogger<ListArchivesHandler> logger
) : IHandler
{
    public async Task<Result<IEnumerable<TypeArchiveListItem>>> Handle()
    {
        await Task.Yield();

        var mapped = typeArchiveStorage.TypeArchives.Select(x => new TypeArchiveListItem(
                x.Namespace,
                x.AbsoluteFilePath,
                x.IsEmbedded,
                x.Types.Count
            )
        );

        return Result.Ok(mapped);
    }
}
