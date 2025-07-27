using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class GetBinaryLengthHandler(
    UserRuntimeProjectRepository repository,
    ILogger<GetBinaryLengthHandler> logger
) : IHandler
{
    public async Task<Result<long>> Handle(Guid projectId, Guid programId)
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

        return Result.Ok(program.FileLength);
    }
}
