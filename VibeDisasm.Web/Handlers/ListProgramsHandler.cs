using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Handlers;

public class ListProgramsHandler(AppState state)
{
    public Task<Result<IEnumerable<ProgramDetailsDto>>> Handle(Guid projectId)
    {
        var project = state.Projects.FirstOrDefault(x => x.Id == projectId);
        if (project is null)
        {
            return Task.FromResult(Result.Fail<IEnumerable<ProgramDetailsDto>>($"Project with ID {projectId} not found"));
        }

        var programDetails = project.Programs.Select(p =>
            new ProgramDetailsDto(p.Id, p.Name, p.FilePath));

        return Task.FromResult(Result.Ok(programDetails));
    }
}
