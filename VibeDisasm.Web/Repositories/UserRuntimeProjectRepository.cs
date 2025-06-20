using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.Repositories;

public class UserRuntimeProjectRepository
{
    private readonly List<RuntimeUserProject> _projects = [];

    public Task<RuntimeUserProject?> GetById(Guid id)
    {
        return Task.FromResult(_projects.FirstOrDefault(x => x.Id == id));
    }

    public Task Add(RuntimeUserProject runtimeProject)
    {
        _projects.Add(runtimeProject);
        return Task.CompletedTask;
    }

    public Task<List<RuntimeUserProject>> GetAll()
    {
        return Task.FromResult(_projects);
    }
}
