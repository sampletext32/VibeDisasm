using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class OpenRecentHandler(
    UserRuntimeProjectRepository repository,
    RecentsService recentsService,
    ProjectArchiveService archiveService,
    ILogger<OpenRecentHandler> logger
) : IHandler
{
    public async Task<Result> Handle(Guid projectId)
    {
        var recent = recentsService.Get()
            .FirstOrDefault(x => x.ProjectId == projectId);

        if (recent is null)
        {
            logger.LogWarning("Recent project {ProjectId} not found", projectId);
            return Result.Fail("Recent project not found");
        }

        var project = await repository.GetById(projectId);

        if (project is not null)
        {
            logger.LogInformation("Opened existing recent project: {ProjectId}", projectId);
            return Result.Ok();
        }

        var loadArchiveResult = await archiveService.Load(recent.Path);

        if (loadArchiveResult.IsFailed)
        {
            logger.LogWarning("Could not load archive for project {ProjectId}. Error: {Error}", projectId, loadArchiveResult.Errors.FirstOrDefault()?.Message);
            return Result.Fail("Failed to load project from archive. " + loadArchiveResult.Errors.FirstOrDefault()?.Message);
        }

        var projectFromArchive = loadArchiveResult.Value;

        await repository.Add(projectFromArchive);

        logger.LogInformation("Opened recent project: {ProjectId} - {ProjectTitle}", projectFromArchive.Id, projectFromArchive.Title);

        return Result.Ok();
    }
}
