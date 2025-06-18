using FluentResults;
using VibeDisasm.Web.ProjectArchive;
using VibeDisasm.Web.Repositories;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class OpenRecentHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly RecentsService _recentsService;
    private readonly ProjectArchiveService _archiveService;
    private readonly ILogger<OpenRecentHandler> _logger;

    public OpenRecentHandler(UserRuntimeProjectRepository repository, RecentsService recentsService, ProjectArchiveService archiveService, ILogger<OpenRecentHandler> logger)
    {
        _repository = repository;
        _recentsService = recentsService;
        _archiveService = archiveService;
        _logger = logger;
    }

    public async Task<Result> Handle(Guid projectId)
    {
        var recent = _recentsService.Get()
            .FirstOrDefault(x => x.ProjectId == projectId);

        if (recent is null)
        {
            _logger.LogWarning("Open recent failed: recent project {ProjectId} not found", projectId);
            return Result.Fail("Recent project not found");
        }

        var project = await _repository.GetById(projectId);

        if (project is not null)
        {
            _logger.LogInformation("Opened existing recent project: {ProjectId}", projectId);
            return Result.Ok();
        }

        var loadArchiveResult = await _archiveService.Load(recent.Path);

        if (loadArchiveResult.IsFailed)
        {
            _logger.LogWarning("Open recent failed: could not load archive for project {ProjectId}. Error: {Error}", projectId, loadArchiveResult.Errors.FirstOrDefault()?.Message);
            return Result.Fail("Failed to load project from archive. " + loadArchiveResult.Errors.FirstOrDefault()?.Message);
        }

        var projectFromArchive = loadArchiveResult.Value;

        await _repository.Add(projectFromArchive);

        _logger.LogInformation("Opened recent project: {ProjectId} - {ProjectTitle}", projectFromArchive.Id, projectFromArchive.Title);

        return Result.Ok();
    }
}
