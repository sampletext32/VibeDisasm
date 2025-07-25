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

        return Result.Ok(program.FileLength);
    }
}
