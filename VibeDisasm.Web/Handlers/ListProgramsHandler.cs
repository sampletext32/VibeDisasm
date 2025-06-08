using System.Diagnostics.Contracts;
using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListProgramsHandler
{
    private readonly UserProgramRepository _repository;

    public ListProgramsHandler(UserProgramRepository repository)
    {
        _repository = repository;
    }

    [Pure]
    public async Task<Result<IEnumerable<ProgramDetailsDto>>> Handle()
    {
        var userPrograms = await _repository.GetAll();

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
