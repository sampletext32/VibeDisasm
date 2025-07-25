using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class CreateProjectHandler(
    UserRuntimeProjectRepository repository,
    ILogger<CreateProjectHandler> logger
) : IHandler
{
    public async Task<Result<Guid>> Handle(CreateProjectDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            logger.LogWarning("Title is empty");
            return Result.Fail("Project title cannot be empty");
        }

        var project = new RuntimeUserProject
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            CreatedAt = DateTime.UtcNow
        };
        await repository.Add(project);

        logger.LogInformation("Created project {ProjectId} - {Title}", project.Id, project.Title);
        return Result.Ok(project.Id);
    }
}
