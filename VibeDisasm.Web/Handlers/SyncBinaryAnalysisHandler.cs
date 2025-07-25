using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Analysis;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class SyncBinaryAnalysisHandler(
    UserRuntimeProjectRepository repository,
    UserProgramDataRepository dataRepository,
    AnalysisExecutor analysisExecutor,
    ILogger<SyncBinaryAnalysisHandler> logger
) : IHandler
{
    public async Task<Result> Handle(Guid projectId, Guid programId)
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

        var result = await analysisExecutor.DoAnalysis(
            new BinaryAnalysisJob(Guid.NewGuid(), project, program, binaryData),
            CancellationToken.None
        );

        return result;
    }
}
