using FluentResults;
using NativeFileDialogSharp;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class SaveProjectHandler(
    ProjectArchiveService archiveService,
    UserRuntimeProjectRepository repository,
    RecentsService recentsService,
    ILogger<SaveProjectHandler> logger
) : IHandler
{
    public async Task<Result> Handle(Guid projectId)
    {
        if (await repository.GetById(projectId) is not {} project)
        {
            logger.ProjectNotFound(projectId);
            return Result.Fail("Project not found");
        }

        if (string.IsNullOrWhiteSpace(project.ProjectArchivePath))
        {
            var dialog = Dialog.FileSave(Constants.ProjectArchiveFileExtension);

            if (dialog.IsOk)
            {
                var path = dialog.Path;

                if (Path.GetExtension(path) is "")
                {
                    path += $".{Constants.ProjectArchiveFileExtension}";
                }

                project.ProjectArchivePath = path;
            }
            else
            {
                logger.LogWarning("File selection was cancelled for project {ProjectId}", projectId);
                return Result.Fail("File selection was cancelled");
            }
        }

        var saveResult = await archiveService.Save(project);

        if (saveResult.IsFailed)
        {
            logger.LogWarning(
                "Could not save archive for project {ProjectId}. Error: {Error}",
                projectId,
                saveResult.Errors.FirstOrDefault()?.Message
            );
            return Result.Fail(
                "Failed to save project archive. " + saveResult.Errors.FirstOrDefault()?.Message
            );
        }

        recentsService.Track(project.ProjectArchivePath);
        logger.LogInformation("Saved project archive for project {ProjectId}", projectId);
        return Result.Ok();
    }
}
