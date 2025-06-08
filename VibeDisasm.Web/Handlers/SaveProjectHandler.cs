using FluentResults;
using NativeFileDialogSharp;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class SaveProjectHandler
{
    private readonly ProjectArchiveService _archiveService;
    private readonly UserRuntimeProjectRepository _repository;
    private readonly RecentsService _recentsService;

    public SaveProjectHandler(ProjectArchiveService archiveService, UserRuntimeProjectRepository repository, RecentsService recentsService)
    {
        _archiveService = archiveService;
        _repository = repository;
        _recentsService = recentsService;
    }

    public async Task<Result> Handle(Guid projectId)
    {
        var runtimeProject = await _repository.GetById(projectId);

        if (runtimeProject is null)
        {
            return Result.Fail("Project not found");
        }

        if (string.IsNullOrWhiteSpace(runtimeProject.ProjectArchivePath))
        {
            var dialog = Dialog.FileSave("vdisa");

            if (dialog.IsOk)
            {
                var path = dialog.Path;

                if (Path.GetExtension(path) is "")
                {
                    path += ".vdisa";
                }

                runtimeProject.ProjectArchivePath = path;
            }
            else
            {
                return Result.Fail("File selection was cancelled");
            }
        }

        await _archiveService.Save(runtimeProject);

        _recentsService.Track(runtimeProject.ProjectArchivePath);

        return Result.Ok();
    }
}
