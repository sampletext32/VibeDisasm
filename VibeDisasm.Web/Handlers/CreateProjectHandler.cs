using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class CreateProjectHandler
{
    private readonly UserProjectRepository _repository;
    public CreateProjectHandler(UserProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(CreateProjectDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Fail<Guid>("Project title cannot be empty");
        }

        var project = new UserProject() {Id = Guid.NewGuid(), Title = request.Title};
        await _repository.Add(project);

        return Result.Ok(project.Id);
    }
}
