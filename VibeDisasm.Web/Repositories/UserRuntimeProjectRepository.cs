using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Repositories;

public class UserRuntimeProjectRepository
{
    private readonly List<UserRuntimeProject> _projects = [];

    public Task<UserRuntimeProject?> GetById(Guid id)
    {
        return Task.FromResult(_projects.FirstOrDefault(x => x.Id == id));
    }

    public Task Add(UserRuntimeProject runtimeProject)
    {
        _projects.Add(runtimeProject);
        return Task.CompletedTask;
    }

    public Task<List<UserRuntimeProject>> GetAll()
    {
        return Task.FromResult(_projects);
    }
}
