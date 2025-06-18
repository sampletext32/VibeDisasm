using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Repositories;
using Microsoft.Extensions.Logging;

namespace VibeDisasm.Web.Handlers;

public class ListingAtAddressHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly ILogger<ListingAtAddressHandler> _logger;

    public ListingAtAddressHandler(UserRuntimeProjectRepository repository, ILogger<ListingAtAddressHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<(UserProgramDatabaseEntry?, JsonSerializerOptions)>> Handle(Guid projectId, Guid programId, uint address)
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

        var entry = program.Database.EntryManager.GetEntryAt(address);
        _logger.LogInformation("Fetched entry at address {Address} in program {ProgramId} of project {ProjectId}", address, programId, projectId);
        return Result.Ok((entry, JsonSerializerOptionsPresets.DatabaseEntryTypeOptions));
    }
}
