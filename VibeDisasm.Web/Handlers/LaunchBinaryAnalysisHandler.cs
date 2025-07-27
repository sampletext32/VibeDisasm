using System.Threading.Channels;
using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class LaunchBinaryAnalysisHandler(
    UserRuntimeProjectRepository repository,
    UserProgramDataRepository dataRepository,
    Channel<BinaryAnalysisJob> channel,
    BackgroundJobRepository backgroundJobRepository,
    ILogger<LaunchBinaryAnalysisHandler> logger
) : IHandler
{
    public async Task<Result<Guid>> Handle(Guid projectId, Guid programId)
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

        var binaryData = await dataRepository.GetOrLoad(program);

        var id = Guid.NewGuid();
        var backgroundJob = new BinaryAnalysisJob(id, project, program, binaryData);

        await backgroundJobRepository.Add(backgroundJob);
        await channel.Writer.WriteAsync(backgroundJob);

        return Result.Ok(id);
    }
}
