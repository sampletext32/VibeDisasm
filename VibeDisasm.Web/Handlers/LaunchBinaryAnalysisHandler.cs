using System.Threading.Channels;
using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class LaunchBinaryAnalysisHandler(
    UserRuntimeProjectRepository repository,
    UserProgramDataRepository dataRepository,
    Channel<BinaryAnalysisBackgroundJob> channel,
    BackgroundJobRepository backgroundJobRepository,
    ILogger<LaunchBinaryAnalysisHandler> logger
) : IHandler
{
    public async Task<Result<Guid>> Handle(Guid projectId, Guid programId)
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

        var binaryData = await dataRepository.GetOrLoad(program);

        var id = Guid.NewGuid();
        var backgroundJob = new BinaryAnalysisBackgroundJob(id, project, program, binaryData);

        await backgroundJobRepository.Add(backgroundJob);
        await channel.Writer.WriteAsync(backgroundJob);

        return Result.Ok(id);
    }
}
