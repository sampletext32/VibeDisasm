using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Overlay;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class AllListingHandler(
    UserRuntimeProjectRepository repository,
    UserProgramDataRepository programDataRepository,
    ILogger<AllListingHandler> logger
) : IHandler
{
    public async Task<Result<(List<InterpretValue2?> interpretedValues, JsonSerializerOptions JsonOptions)>> Handle(
        Guid projectId,
        Guid programId
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

        var entries = program.Database.EntryManager.GetAllEntries();

        var data = await programDataRepository.GetOrLoad(program);

        var interpretedValues = entries.Select(entry => entry switch
                {
                    ArrayUserProgramDatabaseEntry arrayEntry =>
                        TypeInterpreter.InterpretDefault2(
                            OverlayHelper.OverlayArray(arrayEntry.Type, data, (int)entry.Address)
                        ),
                    PrimitiveUserProgramDatabaseEntry primitiveEntry =>
                        TypeInterpreter.InterpretDefault2(
                            OverlayHelper.OverlayPrimitive(primitiveEntry.Type, data, (int)entry.Address)
                        ),
                    StructUserProgramDatabaseEntry structEntry =>
                        TypeInterpreter.InterpretDefault2(
                            OverlayHelper.OverlayStructure(structEntry.Type, data, (int)entry.Address)
                        ),
                    UndefinedUserProgramDatabaseEntry undefinedEntry => null,
                    null => null,
                    _ => throw new ArgumentOutOfRangeException(nameof(entry))
                }
            )
            .ToList();

        logger.LogInformation(
            "Fetched all entries in program {ProgramId} of project {ProjectId}",
            programId,
            projectId
        );
        return Result.Ok((interpretedValues, JsonOptions: JsonSerializerOptionsPresets.StandardOptions));
    }
}
