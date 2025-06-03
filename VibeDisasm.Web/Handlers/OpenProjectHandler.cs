using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class OpenProjectHandler
{
    private readonly UserProjectRepository _repository;
    private readonly AppState _state;
    public OpenProjectHandler(UserProjectRepository repository, AppState state)
    {
        _repository = repository;
        _state = state;
    }

    public async Task<Result> Handle(Guid projectId)
    {
        var project = await _repository.GetById(projectId);

        if (project is null)
        {
            return Result.Fail(new Error("Project not found"));
        }

        if (_state.ActiveProject is not null)
        {
            // TODO: unload active project if it exists
        }

        _state.ActiveProject = project;

        return Result.Ok();
    }
}
