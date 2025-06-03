using System.Diagnostics.Contracts;
using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListProgramsHandler
{
    private readonly UserProgramRepository _repository;
    private readonly AppState _state;

    public ListProgramsHandler(AppState state, UserProgramRepository repository)
    {
        _state = state;
        _repository = repository;
    }

    [Pure]
    public async Task<Result<IEnumerable<ProgramDetailsDto>>> Handle()
    {
        var userPrograms = await _repository.GetAll();
        if (_state.ActiveProject is null)
        {
            return Result.Fail("Active project is not set.");
        }

        var programDetails = userPrograms.Select(
            p => new ProgramDetailsDto(
                p.Id,
                p.Name,
                p.FilePath
            )
        );

        return Result.Ok(programDetails);
    }
}
