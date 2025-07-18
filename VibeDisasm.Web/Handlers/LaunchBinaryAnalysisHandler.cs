using System.Threading.Channels;
using FluentResults;
using VibeDisasm.Web.BackgroundServices.BackgroundJobs;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class LaunchBinaryAnalysisHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly UserProgramDataRepository _dataRepository;
    private readonly Channel<BinaryAnalysisBackgroundJob> _channel;
    private readonly BackgroundJobRepository _backgroundJobRepository;
    private readonly ILogger<LaunchBinaryAnalysisHandler> _logger;

    public LaunchBinaryAnalysisHandler(
        UserRuntimeProjectRepository repository,
        UserProgramDataRepository dataRepository,
        Channel<BinaryAnalysisBackgroundJob> channel,
        BackgroundJobRepository backgroundJobRepository,
        ILogger<LaunchBinaryAnalysisHandler> logger
    )
    {
        _repository = repository;
        _dataRepository = dataRepository;
        _channel = channel;
        _backgroundJobRepository = backgroundJobRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(Guid projectId, Guid programId)
    {
        var project = await _repository.GetById(projectId);

        if (project is null)
        {
            _logger.LogWarning("Launch binary analysis failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);

        if (program is null)
        {
            _logger.LogWarning(
                "Launch binary analysis failed: program {ProgramId} not found in project {ProjectId}",
                programId,
                projectId
            );
            return Result.Fail("Program not found");
        }

        var binaryData = await _dataRepository.GetOrLoad(program);

        var id = Guid.NewGuid();
        var backgroundJob = new BinaryAnalysisBackgroundJob(id, project, program, binaryData);

        await _backgroundJobRepository.Add(backgroundJob);
        await _channel.Writer.WriteAsync(backgroundJob);

        return Result.Ok(id);
    }
}
