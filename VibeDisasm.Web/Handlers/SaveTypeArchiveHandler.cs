using FluentResults;
using NativeFileDialogSharp;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class SaveTypeArchiveHandler(
    ITypeArchiveStorage typeArchiveStorage,
    TypeArchiveService typeArchiveService,
    ILogger<SaveTypeArchiveHandler> logger
) : IHandler
{
    public async Task<Result> Handle(
        string archiveNamespace
    )
    {
        var archive = typeArchiveStorage.FindArchive(archiveNamespace);
        if (archive is null)
        {
            logger.LogWarning(
                "Archive {ArchiveNamespace} not found",
                archiveNamespace
            );
            return Result.Fail("Type archive not found");
        }

        if (archive.IsEmbedded)
        {
            logger.LogWarning(
                "Archive {ArchiveNamespace} is embedded and cannot be saved",
                archiveNamespace
            );
            return Result.Fail("Type archive is embedded and cannot be saved to a file");
        }

        if (string.IsNullOrWhiteSpace(archive.AbsoluteFilePath))
        {
            var dialog = Dialog.FileSave(Constants.TypeArchiveFileExtension);

            if (dialog.IsOk)
            {
                var path = dialog.Path;

                if (Path.GetExtension(path) is "")
                {
                    path += $".{Constants.TypeArchiveFileExtension}";
                }

                archive.AbsoluteFilePath = path;
            }
            else
            {
                logger.LogWarning(
                    "File selection was cancelled for type-archive {ArchiveNamespace}",
                    archiveNamespace
                );
                return Result.Fail("File selection was cancelled");
            }
        }

        await typeArchiveService.SaveTypeArchive(archive);

        logger.LogInformation(
            "Successfully saved type archive {ArchiveNamespace} to {Path}",
            archiveNamespace,
            archive.AbsoluteFilePath
        );
        return Result.Ok();
    }
}
