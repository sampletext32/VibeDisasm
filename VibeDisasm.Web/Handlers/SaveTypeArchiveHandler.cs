using FluentResults;
using NativeFileDialogSharp;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class SaveTypeArchiveHandler(
    UserRuntimeProjectRepository repository,
    TypeArchiveService typeArchiveService,
    ILogger<SaveTypeArchiveHandler> logger
) : IHandler
{
    public async Task<Result> Handle(
        Guid projectId,
        Guid programId,
        string archiveNamespace
    )
    {
        var project = await repository.GetById(projectId);
        if (project is null)
        {
            logger.ProjectNotFound(projectId);
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);

        if (program is null)
        {
            logger.ProgramNotFound(programId, projectId);
            return Result.Fail("Program not found");
        }

        var archive = program.Database.TypeStorage.Archives.FirstOrDefault(x => x.Namespace == archiveNamespace);
        if (archive is null)
        {
            logger.LogWarning(
                "Save type archive failed: archive {ArchiveNamespace} not found in program {ProgramId}",
                archiveNamespace,
                programId
            );
            return Result.Fail("Type archive not found");
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
                    "Save cancelled: file selection was cancelled for type-archive {ArchiveNamespace}",
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
