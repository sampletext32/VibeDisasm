using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Repositories;
using Microsoft.Extensions.Logging;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Handlers;

public class ListArchivesHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly ILogger<ListArchivesHandler> _logger;

    public ListArchivesHandler(UserRuntimeProjectRepository repository, ILogger<ListArchivesHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<TypeArchiveListItem>>> Handle(Guid projectId, Guid programId)
    {
        var project = await _repository.GetById(projectId);
        if (project is null)
        {
            _logger.LogWarning("Get entry at address failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);

        if (program is null)
        {
            _logger.LogWarning("Get entry at address failed: program {ProgramId} not found in project {ProjectId}",
                programId, projectId);
            return Result.Fail("Program not found");
        }

        var mapped = program.Database.TypeStorage.Archives
            .Select(x => new TypeArchiveListItem(
                x.Namespace,
                x.AbsoluteFilePath,
                x.Types.Count
            ));

        return Result.Ok(mapped);
    }
}
