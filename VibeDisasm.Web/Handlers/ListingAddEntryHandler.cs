using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListingAddEntryHandler(
    UserRuntimeProjectRepository repository,
    ILogger<ListingAddEntryHandler> logger
) : IHandler
{
    public async Task<Result> Handle(Guid projectId, Guid programId, UserProgramDatabaseEntry entry)
    {
        if (await repository.GetById(projectId) is not {} project)
        {
            logger.ProjectNotFound(projectId);
            return Result.Fail("Project not found");
        }

        if (project.GetProgram(programId) is not { } program)
        {
            logger.ProgramNotFound(programId, projectId);
            return Result.Fail("Program not found");
        }

        program.Database.EntryManager.AddEntry(entry);
        logger.LogInformation(
            "Added entry at address {Address} to program {ProgramId} in project {ProjectId}",
            entry.Address,
            programId,
            projectId
        );
        return Result.Ok();
    }
}
