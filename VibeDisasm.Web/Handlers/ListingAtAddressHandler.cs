using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Overlay;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListingAtAddressHandler(
    UserRuntimeProjectRepository repository,
    UserProgramDataRepository programDataRepository,
    ILogger<ListingAtAddressHandler> logger
) : IHandler
{
    public async Task<Result<(InterpretedValue? interpretedValue, JsonSerializerOptions DatabaseEntryOptions)>> Handle(
        Guid projectId,
        Guid programId,
        uint address
    )
    {
        if (await repository.GetById(projectId) is not {} project)
        {
            logger.ProjectNotFound(projectId);
            return Result.Fail("Project not found");
        }

        if (project.GetProgram(programId) is not {} program)
        {
            logger.ProgramNotFound(programId, projectId);
            return Result.Fail("Program not found");
        }

        var entry = program.Database.EntryManager.GetEntryAt(address);

        var data = await programDataRepository.GetOrLoad(program);

        var interpretedValue = entry switch
        {
            ArrayUserProgramDatabaseEntry arrayEntry =>
                TypeInterpreter.Interpret(OverlayHelper.OverlayArray(program, arrayEntry.Type, data, (int)entry.Address)),
            PrimitiveUserProgramDatabaseEntry primitiveEntry =>
                TypeInterpreter.Interpret(OverlayHelper.OverlayPrimitive(program, primitiveEntry.Type, data, (int)entry.Address)),
            StructUserProgramDatabaseEntry structEntry =>
                TypeInterpreter.Interpret(OverlayHelper.OverlayStructure(program, structEntry.Type, data, (int)entry.Address)),
            UndefinedUserProgramDatabaseEntry undefinedEntry => null,
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };


        logger.LogInformation(
            "Fetched entry at address {Address} in program {ProgramId} of project {ProjectId}",
            address,
            programId,
            projectId
        );
        return Result.Ok((interpretedValue, JsonSerializerOptionsPresets.StandardOptions));
    }
}
