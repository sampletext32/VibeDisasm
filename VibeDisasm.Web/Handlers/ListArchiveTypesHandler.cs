using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Repositories;

namespace VibeDisasm.Web.Handlers;

public class ListArchiveTypesHandler(
    UserRuntimeProjectRepository repository,
    ILogger<ListArchiveTypesHandler> logger
) : IHandler
{
    public async Task<Result<(List<TypeArchiveElementDto> types, JsonSerializerOptions SerializerOptions)>> Handle(
        Guid projectId,
        Guid programId,
        string archiveNamespace
    )
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

        var archive = program.Database.TypeStorage.Archives.FirstOrDefault(x => x.Namespace == archiveNamespace);

        if (archive is null)
        {
            logger.LogWarning(
                "Archive {ArchiveNamespace} not found for program {ProgramId} in project {ProjectId}",
                archiveNamespace,
                programId,
                projectId
            );
            return Result.Fail("Archive not found");
        }

        var types = new List<TypeArchiveElementDto>(archive.Types.Count);
        types.AddRange(
            archive.Types.Select(databaseType => CreateTypeArchiveElementDto(databaseType, program.Database.TypeStorage)
            )
        );

        return Result.Ok((types, JsonSerializerOptionsPresets.TypeArchiveElementOptions));
    }

    private static TypeArchiveElementDto CreateTypeArchiveElementDto(
        RuntimeDatabaseType databaseType,
        RuntimeTypeStorage storage
    )
    {
        return databaseType switch
        {
            RuntimeArrayType type => new TypeArchiveArrayElementDto(
                type.Id,
                CreateTypeArchiveElementDto(type.ElementType, storage),
                type.ElementCount
            ),
            RuntimeFunctionType type => new TypeArchiveFunctionElementDto(
                type.Id,
                type.Name,
                CreateTypeArchiveElementDto(type.ReturnType, storage),
                type.Arguments.Select(x => new TypeArchiveFunctionArgumentElementDto(
                        CreateTypeArchiveElementDto(x.Type, storage),
                        x.Name
                    )
                )
            ),
            RuntimePointerType type => new TypeArchivePointerElementDto(
                type.Id,
                CreateTypeArchiveElementDto(type.PointedType, storage)
            ),
            RuntimePrimitiveType type => new TypeArchivePrimitiveElementDto(
                type.Id,
                type.Name,
                type.Size
            ),
            RuntimeStructureType type => new TypeArchiveStructureElementDto(
                type.Id,
                type.Name,
                type.Fields.Select(x => new TypeArchiveStructureFieldElementDto(
                        CreateTypeArchiveElementDto(x.Type, storage),
                        x.Name
                    )
                )
            ),
            RuntimeTypeRefType type => new TypeArchiveTypeRefElementDto(
                type.Id,
                CreateTypeArchiveElementDto(
                    storage.ResolveTypeRef(type)
                    ?? new RuntimeTypeRefType(Guid.Empty, "unresolved"),
                    storage
                )
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }
}
