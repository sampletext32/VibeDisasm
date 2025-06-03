using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ImportProgramHandler
{
    private readonly UserProgramRepository _repository;
    private readonly AppState _state;

    public ImportProgramHandler(UserProgramRepository repository, AppState state)
    {
        _repository = repository;
        _state = state;
    }

    public async Task<Result<Guid>> Handle()
    {
        if (_state.ActiveProject is null)
        {
            return Result.Fail("No project is opened");
        }

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
