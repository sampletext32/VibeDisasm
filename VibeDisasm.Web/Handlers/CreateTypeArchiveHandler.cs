using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class CreateTypeArchiveHandler(
    UserRuntimeProjectRepository repository,
    ILogger<CreateTypeArchiveHandler> logger
) : IHandler
{
    public async Task<Result> Handle(Guid projectId, Guid programId, string archiveNamespace)
    {
        var project = await repository.GetById(projectId);
        if (project is null)
        {
            logger.ProjectNotFound(projectId);
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);
        if (program is null)
        {
            logger.ProgramNotFound(programId, projectId);
            return Result.Fail("Program not found");
        }

        // Check if archive with the same namespace already exists
        if (program.Database.TypeStorage.Archives.Any(x => x.Namespace == archiveNamespace))
        {
            logger.LogWarning(
                "Archive with namespace {ArchiveNamespace} already exists",
                archiveNamespace
            );
            return Result.Fail($"Type archive with namespace '{archiveNamespace}' already exists");
        }

        try
        {
            // user created archives are not embedded by design
            var newArchive = new RuntimeTypeArchive(archiveNamespace, isEmbedded: false);
            program.Database.TypeStorage.Archives.Add(newArchive);

            logger.LogInformation(
                "Successfully created type archive {ArchiveNamespace} for program {ProgramId}",
                archiveNamespace,
                programId
            );
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create type archive {ArchiveNamespace}", archiveNamespace);
            return Result.Fail($"Failed to create type archive: {ex.Message}");
        }
    }
}
