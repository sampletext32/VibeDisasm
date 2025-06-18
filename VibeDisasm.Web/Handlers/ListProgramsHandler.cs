using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListProgramsHandler
{
    private readonly UserRuntimeProjectRepository _repository;

    public ListProgramsHandler(UserRuntimeProjectRepository repository)
    {
        _repository = repository;
    }

    [Pure]
    public async Task<Result<IEnumerable<ProgramDetailsDto>>> Handle(Guid projectId)
    {
        var project = await _repository.GetById(projectId);

        if (project is null)
        {
            return Result.Fail("Project not found");
        }

        var userPrograms = project.Programs;

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
