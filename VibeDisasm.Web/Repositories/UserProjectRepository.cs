using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Repositories;

public class UserProjectRepository
{
    private readonly List<UserProject> _projects = [];

    public Task<UserProject?> GetById(Guid id)
    {
        return Task.FromResult(_projects.FirstOrDefault(x => x.Id == id));
    }

    public Task Add(UserProject project)
    {
        _projects.Add(project);
        return Task.CompletedTask;
    }

    public Task<List<UserProject>> GetAll()
    {
        return Task.FromResult(_projects);
    }
}
