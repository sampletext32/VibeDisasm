using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListProgramsHandler(
    UserRuntimeProjectRepository repository,
    ILogger<ListProgramsHandler> logger
) : IHandler
{
    [Pure]
    public async Task<Result<IEnumerable<ProgramDetailsDto>>> Handle(Guid projectId)
    {
        var project = await repository.GetById(projectId);

        if (project is null)
        {
            logger.ProjectNotFound(projectId);
            return Result.Fail("Project not found");
        }

        var userPrograms = project.Programs;
        var programDetails = userPrograms.Select(p => new ProgramDetailsDto(
                p.Id,
                p.Name,
                p.FilePath,
                p.FileLength,
                p.Kind,
                p.Architecture
            )
        );
        logger.LogInformation("Listed programs for project {ProjectId}", projectId);
        return Result.Ok(programDetails);
    }
}
