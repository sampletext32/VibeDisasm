using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Handlers;

public class ListProjectsHandler(AppState state)
{
    public Task<Result<IEnumerable<ProjectDetailsDto>>> Handle()
    {
        var projectDetails = state.Projects.Select(p => new ProjectDetailsDto(p.Id, p.Title));
        return Task.FromResult(Result.Ok(projectDetails));
    }
}
