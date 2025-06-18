using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Repositories;
using Microsoft.Extensions.Logging;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Handlers;

public class ListTypesHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly ILogger<ListTypesHandler> _logger;

    public ListTypesHandler(UserRuntimeProjectRepository repository, ILogger<ListTypesHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<(List<DatabaseType>, JsonSerializerOptions)>> Handle(Guid projectId, Guid programId)
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
            _logger.LogWarning("Get entry at address failed: program {ProgramId} not found in project {ProjectId}", programId, projectId);
            return Result.Fail("Program not found");
        }

        var types = program.Database.TypeStorage.Types;
        return Result.Ok((types, JsonSerializerOptionsPresets.DatabaseTypeOptions));
    }
}
