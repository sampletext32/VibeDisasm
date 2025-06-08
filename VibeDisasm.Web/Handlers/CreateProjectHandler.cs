using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class CreateProjectHandler
{
    private readonly UserRuntimeProjectRepository _repository;

    public CreateProjectHandler(UserRuntimeProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(CreateProjectDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Fail<Guid>("Project title cannot be empty");
        }

        var project = new UserRuntimeProject() {Id = Guid.NewGuid(), Title = request.Title, CreatedAt = DateTime.UtcNow};
        await _repository.Add(project);

        return Result.Ok(project.Id);
    }
}
