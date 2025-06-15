using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListingAtAddressHandler
{
    private readonly UserRuntimeProjectRepository _repository;

    public ListingAtAddressHandler(UserRuntimeProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ListingElementDto>> Handle(Guid projectId, Guid programId, uint address)
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

        var entry = program.Database.EntryManager.GetEntryAt(address);

        if (entry is null)
        {
            return Result.Ok(new ListingElementDto());
        }

        return Result.Ok(new ListingElementDto());
    }
}
