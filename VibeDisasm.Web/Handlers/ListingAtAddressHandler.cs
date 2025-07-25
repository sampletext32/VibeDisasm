using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListingAtAddressHandler(
    UserRuntimeProjectRepository repository,
    ILogger<ListingAtAddressHandler> logger
) : IHandler
{
    public async Task<Result<(UserProgramDatabaseEntry?, JsonSerializerOptions)>> Handle(
        Guid projectId,
        Guid programId,
        uint address
    )
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

        var entry = program.Database.EntryManager.GetEntryAt(address);
        logger.LogInformation(
            "Fetched entry at address {Address} in program {ProgramId} of project {ProjectId}",
            address,
            programId,
            projectId
        );
        return Result.Ok((entry, DatabaseEntryTypeOptions: JsonSerializerOptionsPresets.DatabaseEntryOptions));
    }
}
