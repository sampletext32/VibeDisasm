using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class CreateTypeArchiveHandler(
    ITypeArchiveStorage typeArchiveStorage,
    ILogger<CreateTypeArchiveHandler> logger
) : IHandler
{
    public async Task<Result> Handle(string archiveNamespace)
    {
        await Task.Yield();

        // Check if archive with the same namespace already exists
        if (typeArchiveStorage.FindArchive(archiveNamespace) is not null)
        {
            logger.LogWarning(
                "Archive with namespace {ArchiveNamespace} already exists",
                archiveNamespace
            );
            return Result.Fail($"Type archive with namespace '{archiveNamespace}' already exists");
        }

        try
        {
            // user created archives are not embedded by design
            var newArchive = new RuntimeTypeArchive(archiveNamespace, isEmbedded: false);
            typeArchiveStorage.Import(newArchive);

            logger.LogInformation(
                "Successfully created type archive {ArchiveNamespace}",
                archiveNamespace
            );
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create type archive {ArchiveNamespace}", archiveNamespace);
            return Result.Fail($"Failed to create type archive: {ex.Message}");
        }
    }
}
