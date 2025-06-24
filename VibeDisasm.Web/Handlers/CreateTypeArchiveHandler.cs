using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class CreateTypeArchiveHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly ILogger<CreateTypeArchiveHandler> _logger;

    public CreateTypeArchiveHandler(
        UserRuntimeProjectRepository repository,
        ILogger<CreateTypeArchiveHandler> logger
    )
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result> Handle(Guid projectId, Guid programId, string archiveNamespace)
    {
        var project = await _repository.GetById(projectId);
        if (project is null)
        {
            _logger.LogWarning("Create type archive failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);
        if (program is null)
        {
            _logger.LogWarning(
                "Create type archive failed: program {ProgramId} not found in project {ProjectId}",
                programId,
                projectId
            );
            return Result.Fail("Program not found");
        }

        // Check if archive with the same namespace already exists
        if (program.Database.TypeStorage.Archives.Any(x => x.Namespace == archiveNamespace))
        {
            _logger.LogWarning(
                "Create type archive failed: archive with namespace {ArchiveNamespace} already exists",
                archiveNamespace
            );
            return Result.Fail($"Type archive with namespace '{archiveNamespace}' already exists");
        }

        try
        {
            var newArchive = new RuntimeTypeArchive(archiveNamespace);
            program.Database.TypeStorage.Archives.Add(newArchive);

            _logger.LogInformation(
                "Successfully created type archive {ArchiveNamespace} for program {ProgramId}",
                archiveNamespace,
                programId
            );
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create type archive {ArchiveNamespace}", archiveNamespace);
            return Result.Fail($"Failed to create type archive: {ex.Message}");
        }
    }
}
