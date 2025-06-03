using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Handlers;

public class CreateProjectHandler(AppState state)
{
    public Task<Guid> Handle(CreateProjectDto request)
    {
        var project = new UserProject() {Id = Guid.NewGuid(), Title = request.Title ?? ""};

        state.Projects.Add(project);

        return Task.FromResult(project.Id);
    }
}
