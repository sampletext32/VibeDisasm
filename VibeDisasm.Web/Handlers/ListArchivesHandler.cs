using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListArchivesHandler(
    UserRuntimeProjectRepository repository,
    ILogger<ListArchivesHandler> logger
) : IHandler
{
    public async Task<Result<IEnumerable<TypeArchiveListItem>>> Handle(Guid projectId, Guid programId)
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

        var mapped = program.Database.TypeStorage.Archives.Select(x => new TypeArchiveListItem(
                x.Namespace,
                x.AbsoluteFilePath,
                x.Types.Count
            )
        );

        return Result.Ok(mapped);
    }
}
