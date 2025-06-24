using FluentResults;
using Microsoft.Extensions.Logging;
using NativeFileDialogSharp;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class SaveTypeArchiveHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly TypeArchiveService _typeArchiveService;
    private readonly ILogger<SaveTypeArchiveHandler> _logger;

    public SaveTypeArchiveHandler(
        UserRuntimeProjectRepository repository,
        TypeArchiveService typeArchiveService,
        ILogger<SaveTypeArchiveHandler> logger
    )
    {
        _repository = repository;
        _typeArchiveService = typeArchiveService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        Guid projectId,
        Guid programId,
        string archiveNamespace
    )
    {
        var project = await _repository.GetById(projectId);
        if (project is null)
        {
            _logger.LogWarning("Save type archive failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);
        if (program is null)
        {
            _logger.LogWarning(
                "Save type archive failed: program {ProgramId} not found in project {ProjectId}",
                programId,
                projectId
            );
            return Result.Fail("Program not found");
        }

        var archive = program.Database.TypeStorage.Archives.FirstOrDefault(x => x.Namespace == archiveNamespace);
        if (archive is null)
        {
            _logger.LogWarning(
                "Save type archive failed: archive {ArchiveNamespace} not found in program {ProgramId}",
                archiveNamespace,
                programId
            );
            return Result.Fail("Type archive not found");
        }

        if (string.IsNullOrWhiteSpace(archive.AbsoluteFilePath))
        {
            var dialog = Dialog.FileSave("vdisa");

            if (dialog.IsOk)
            {
                var path = dialog.Path;

                if (Path.GetExtension(path) is "")
                {
                    path += ".vtarc";
                }

                archive.AbsoluteFilePath = path;
            }
            else
            {
                _logger.LogWarning(
                    "Save cancelled: file selection was cancelled for type-archive {ArchiveNamespace}",
                    archiveNamespace
                );
                return Result.Fail("File selection was cancelled");
            }
        }

        await _typeArchiveService.SaveTypeArchive(archive);

        _logger.LogInformation(
            "Successfully saved type archive {ArchiveNamespace} to {Path}",
            archiveNamespace,
            archive.AbsoluteFilePath
        );
        return Result.Ok();
    }
}
