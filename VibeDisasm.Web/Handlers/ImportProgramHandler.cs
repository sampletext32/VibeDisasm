using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;
using Microsoft.Extensions.Logging;

namespace VibeDisasm.Web.Handlers;

public class ImportProgramHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly UserProgramDataRepository _dataRepository;
    private readonly ILogger<ImportProgramHandler> _logger;

    public ImportProgramHandler(UserRuntimeProjectRepository repository, UserProgramDataRepository dataRepository, ILogger<ImportProgramHandler> logger)
    {
        _repository = repository;
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(Guid projectId)
    {
        var project = await _repository.GetById(projectId);

        if (project is null)
        {
            _logger.LogWarning("Import failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        var dialog = NativeFileDialogSharp.Dialog.FileOpen("dll,exe");

        if (dialog.IsOk)
        {
            var filePath = dialog.Path;
            var fileLength = new FileInfo(filePath).Length;

            if (fileLength > int.MaxValue)
            {
                _logger.LogWarning("Import failed: file {FilePath} too large", filePath);
                throw new InvalidOperationException("File size too large");
            }

            var program = new UserProgram(Guid.NewGuid(), filePath, Path.GetFileName(filePath), fileLength);
            project.Programs.Add(program);
            _logger.LogInformation("Imported program {ProgramId} into project {ProjectId}", program.Id, projectId);
            return Result.Ok(program.Id);
        }
        else
        {
            _logger.LogWarning("Import cancelled by user for project {ProjectId}", projectId);
            return Result.Fail("File selection was cancelled");
        }
    }
}
