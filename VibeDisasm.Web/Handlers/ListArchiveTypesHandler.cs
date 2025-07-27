using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Abstractions;
using VibeDisasm.Web.Dtos;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.Handlers;

public class ListArchiveTypesHandler(
    ITypeArchiveStorage typeArchiveStorage,
    ILogger<ListArchiveTypesHandler> logger
) : IHandler
{
    public async Task<Result<(List<TypeArchiveElementDto> types, JsonSerializerOptions SerializerOptions)>> Handle(
        string archiveNamespace
    )
    {
        await Task.Yield();

        if (typeArchiveStorage.FindArchive(archiveNamespace) is not {} archive)
        {
            logger.LogWarning(
                "Archive {ArchiveNamespace} not found",
                archiveNamespace
            );
            return Result.Fail("Archive not found");
        }

        var types = new List<TypeArchiveElementDto>(archive.Types.Count);
        types.AddRange(
            archive.Types.Select(CreateTypeArchiveElementDto)
        );

        return Result.Ok((types, JsonSerializerOptionsPresets.StandardOptions));
    }

    private static TypeArchiveElementDto CreateTypeArchiveElementDto(
        RuntimeDatabaseType databaseType
    )
    {
        return databaseType switch
        {
            RuntimeArrayType type => new TypeArchiveArrayElementDto(
                type.Id,
                CreateTypeArchiveElementDto(type.ElementType),
                type.ElementCount
            ),
            RuntimeFunctionType type => new TypeArchiveFunctionElementDto(
                type.Id,
                type.Name,
                CreateTypeArchiveElementDto(type.ReturnType),
                type.Arguments.Select(x => new TypeArchiveFunctionArgumentElementDto(
                        CreateTypeArchiveElementDto(x.Type),
                        x.Name
                    )
                )
            ),
            RuntimePointerType type => new TypeArchivePointerElementDto(
                type.Id,
                CreateTypeArchiveElementDto(type.PointedType)
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
                        CreateTypeArchiveElementDto(x.Type),
                        x.Name
                    )
                )
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType))
        };
    }
}
