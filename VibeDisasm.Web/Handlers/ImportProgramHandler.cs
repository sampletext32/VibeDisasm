using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ImportProgramHandler(
    UserRuntimeProjectRepository repository,
    ILogger<ImportProgramHandler> logger
) : IHandler
{
    public async Task<Result<Guid>> Handle(Guid projectId)
    {
        var project = await repository.GetById(projectId);

        if (project is null)
        {
            logger.ProjectNotFound(projectId);
            return Result.Fail("Project not found");
        }

        var dialog = NativeFileDialogSharp.Dialog.FileOpen("dll,exe");

        if (dialog.IsOk)
        {
            var filePath = dialog.Path;
            var fileLength = new FileInfo(filePath).Length;

            if (fileLength > int.MaxValue)
            {
                logger.LogWarning("File {FilePath} too large", filePath);
                throw new InvalidOperationException("File size too large");
            }

            var program = new RuntimeUserProgram(Guid.NewGuid(), filePath, Path.GetFileName(filePath), fileLength);
            project.Programs.Add(program);
            logger.LogInformation("Imported program {ProgramId} into project {ProjectId}", program.Id, projectId);
            return Result.Ok(program.Id);
        }
        else
        {
            logger.LogWarning("Import cancelled by user for project {ProjectId}", projectId);
            return Result.Fail("File selection was cancelled");
        }
    }
}
