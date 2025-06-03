using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Handlers;

public class CreateProjectHandler(AppState state)
{
    public Task<Result<Guid>> Handle(CreateProjectDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Task.FromResult(Result.Fail<Guid>("Project title cannot be empty"));
        }

        var project = new UserProject() {Id = Guid.NewGuid(), Title = request.Title};
        state.Projects.Add(project);

        return Task.FromResult(Result.Ok(project.Id));
    }
}
