using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;
using Microsoft.Extensions.Logging;

namespace VibeDisasm.Web.Handlers;

public class ListProgramsHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly ILogger<ListProgramsHandler> _logger;

    public ListProgramsHandler(UserRuntimeProjectRepository repository, ILogger<ListProgramsHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [Pure]
    public async Task<Result<IEnumerable<ProgramDetailsDto>>> Handle(Guid projectId)
    {
        var project = await _repository.GetById(projectId);

        if (project is null)
        {
            _logger.LogWarning("List programs failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        var userPrograms = project.Programs;
        var programDetails = userPrograms.Select(
            p => new ProgramDetailsDto(
                p.Id,
                p.Name,
                p.FilePath,
                p.FileLength,
                p.Kind,
                p.Architecture
            )
        );
        _logger.LogInformation("Listed programs for project {ProjectId}", projectId);
        return Result.Ok(programDetails);
    }
}
