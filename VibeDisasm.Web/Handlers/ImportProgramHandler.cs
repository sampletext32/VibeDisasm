using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ImportProgramHandler
{
    private readonly UserProgramRepository _repository;

    public ImportProgramHandler(UserProgramRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle()
    {
        var dialog = NativeFileDialogSharp.Dialog.FileOpen("dll,exe");

        if (dialog.IsOk)
        {
            var filePath = dialog.Path;

            var program = new UserProgram(Guid.NewGuid(), filePath, Path.GetFileName(filePath));

            await _repository.Add(program);
            return Result.Ok(program.Id);
        }
        else
        {
            return Result.Fail("File selection was cancelled");
        }
    }
}
