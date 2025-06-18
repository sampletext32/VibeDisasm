using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class CreateProjectHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly ILogger<CreateProjectHandler> _logger;

    public CreateProjectHandler(UserRuntimeProjectRepository repository, ILogger<CreateProjectHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateProjectDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            _logger.LogWarning("Failed to create project: title is empty");
            return Result.Fail<Guid>("Project title cannot be empty");
        }

        var project = new UserRuntimeProject { Id = Guid.NewGuid(), Title = request.Title, CreatedAt = DateTime.UtcNow };
        await _repository.Add(project);
        _logger.LogInformation("Created project {ProjectId} - {Title}", project.Id, project.Title);
        return Result.Ok(project.Id);
    }
}
