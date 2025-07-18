using FluentResults;
using NativeFileDialogSharp;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;
using Microsoft.Extensions.Logging;

namespace VibeDisasm.Web.Handlers;

public class SaveProjectHandler
{
    private readonly ProjectArchiveService _archiveService;
    private readonly UserRuntimeProjectRepository _repository;
    private readonly RecentsService _recentsService;
    private readonly ILogger<SaveProjectHandler> _logger;

    public SaveProjectHandler(ProjectArchiveService archiveService, UserRuntimeProjectRepository repository, RecentsService recentsService, ILogger<SaveProjectHandler> logger)
    {
        _archiveService = archiveService;
        _repository = repository;
        _recentsService = recentsService;
        _logger = logger;
    }

    public async Task<Result> Handle(Guid projectId)
    {
        var runtimeProject = await _repository.GetById(projectId);

        if (runtimeProject is null)
        {
            _logger.LogWarning("Save failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        if (string.IsNullOrWhiteSpace(runtimeProject.ProjectArchivePath))
        {
            var dialog = Dialog.FileSave(Constants.ProjectArchiveFileExtension);

            if (dialog.IsOk)
            {
                var path = dialog.Path;

                if (Path.GetExtension(path) is "")
                {
                    path += $".{Constants.ProjectArchiveFileExtension}";
                }

                runtimeProject.ProjectArchivePath = path;
            }
            else
            {
                _logger.LogWarning("Save cancelled: file selection was cancelled for project {ProjectId}", projectId);
                return Result.Fail("File selection was cancelled");
            }
        }

        var saveResult = await _archiveService.Save(runtimeProject);

        if (saveResult.IsFailed)
        {
            _logger.LogWarning("Save failed: could not save archive for project {ProjectId}. Error: {Error}", projectId, saveResult.Errors.FirstOrDefault()?.Message);
            return Result.Fail(
                "Failed to save project archive. " + saveResult.Errors.FirstOrDefault()
                    ?.Message
            );
        }

        _recentsService.Track(runtimeProject.ProjectArchivePath);
        _logger.LogInformation("Saved project archive for project {ProjectId}", projectId);
        return Result.Ok();
    }
}
