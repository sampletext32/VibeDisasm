using FluentResults;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListingAddEntryHandler
{
    private readonly UserRuntimeProjectRepository _repository;
    private readonly ILogger<ListingAddEntryHandler> _logger;

    public ListingAddEntryHandler(UserRuntimeProjectRepository repository, ILogger<ListingAddEntryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result> Handle(Guid projectId, Guid programId, UserProgramDatabaseEntry entry)
    {
        var project = await _repository.GetById(projectId);
        if (project is null)
        {
            _logger.LogWarning("Add entry failed: project {ProjectId} not found", projectId);
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);

        if (program is null)
        {
            _logger.LogWarning("Add entry failed: program {ProgramId} not found in project {ProjectId}", programId, projectId);
            return Result.Fail("Program not found");
        }

        program.Database.EntryManager.AddEntry(entry);
        _logger.LogInformation("Added entry at address {Address} to program {ProgramId} in project {ProjectId}", entry.Address, programId, projectId);
        return Result.Ok();
    }
}
