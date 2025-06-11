using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ImportProgramHandler
{
    private readonly UserRuntimeProjectRepository _repository;

    public ImportProgramHandler(UserRuntimeProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(Guid projectId)
    {
        var project = await _repository.GetById(projectId);

        if (project is null)
        {
            return Result.Fail("Project not found");
        }

        var dialog = NativeFileDialogSharp.Dialog.FileOpen("dll,exe");

        if (dialog.IsOk)
        {
            var filePath = dialog.Path;

            var program = new UserProgram(Guid.NewGuid(), filePath, Path.GetFileName(filePath));

            project.Programs.Add(program);

            return Result.Ok(program.Id);
        }
        else
        {
            return Result.Fail("File selection was cancelled");
        }
    }
}
