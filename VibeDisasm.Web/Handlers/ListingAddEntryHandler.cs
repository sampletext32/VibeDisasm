using FluentResults;
using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListingAddEntryHandler
{
    private readonly UserRuntimeProjectRepository _repository;

    public ListingAddEntryHandler(UserRuntimeProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(Guid projectId, Guid programId, UserProgramDatabaseEntry entry)
    {
        var project = await _repository.GetById(projectId);
        if (project is null)
        {
            return Result.Fail("Project not found");
        }

        var program = project.Programs.FirstOrDefault(x => x.Id == programId);

        if (program is null)
        {
            return Result.Fail("Program not found");
        }

        program.Database.EntryManager.AddEntry(entry);

        return Result.Ok();
    }
}
