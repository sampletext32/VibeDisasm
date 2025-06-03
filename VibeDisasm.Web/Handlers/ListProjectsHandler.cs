using System.Diagnostics.Contracts;
using FluentResults;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListProjectsHandler
{
    private readonly UserProjectRepository _repository;

    public ListProjectsHandler(UserProjectRepository repository)
    {
        _repository = repository;
    }

    [Pure]
    public async Task<Result<IEnumerable<ProjectDetailsDto>>> Handle()
    {
        var projects = await _repository.GetAll();
        var mapped = projects.Select(p => new ProjectDetailsDto(p.Id, p.Title, p.CreatedAt));
        return Result.Ok(mapped);
    }
}
