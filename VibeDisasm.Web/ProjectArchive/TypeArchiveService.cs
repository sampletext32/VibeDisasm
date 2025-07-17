using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

namespace VibeDisasm.Web.ProjectArchive;

public class TypeArchiveService(ILogger<TypeArchiveJson> logger)
{
    public async Task LoadAllTypeArchive(Dictionary<string, List<Guid>> programsByTypeArchivePathDict,
        RuntimeUserProject runtimeProject)
    {
        var distinctTypeArchivePaths = programsByTypeArchivePathDict.Keys;

        foreach (var typeArchivePath in distinctTypeArchivePaths)
        {
            var archivePath = typeArchivePath;
            if (Path.IsPathFullyQualified(typeArchivePath))
            {
                logger.LogInformation("Attempting to load type archive from absolute path {TypeArchivePath}",
                    typeArchivePath);
            }
            else
            {
                archivePath = Path.GetFullPath(typeArchivePath);
                logger.LogInformation("Attempting to load type archive from relative path {TypeArchivePath}",
                    typeArchivePath);
            }

            var typeArchive = await LoadTypeArchive(archivePath);

            if (typeArchive is null)
            {
                logger.LogWarning("Failed to load type archive for {TypeArchivePath}. Program may not work.",
                    archivePath);
                continue;
            }

            foreach (var programId in programsByTypeArchivePathDict[archivePath])
            {
                var program = runtimeProject.Programs.FirstOrDefault(x => x.Id == programId) ??
                              throw new InvalidOperationException($"Failed to find program with id {programId}");

                program.Database.TypeStorage.Archives.Add(typeArchive);
            }
        }
    }

    public async Task<RuntimeTypeArchive?> LoadTypeArchive(string typeArchiveAbsolutePath)
    {
        if (!File.Exists(typeArchiveAbsolutePath))
        {
            return null;
        }

        await using var stream = new FileStream(typeArchiveAbsolutePath, FileMode.Open);
        var typeArchiveJson =
            await JsonSerializer.DeserializeAsync<TypeArchiveJson>(stream,
                JsonSerializerOptionsPresets.TypeArchiveJsonOptions);

        if (typeArchiveJson is null)
        {
            logger.LogWarning("TypeArchive is corrupted. Deserialization Failed");
            return null;
        }

        var typeArchive = new RuntimeTypeArchive(typeArchiveJson.Namespace);

        Dictionary<Guid, RuntimeDatabaseType> resolvedTypes = [];

        foreach (var typeArchiveJsonElement in typeArchiveJson.Types)
        {
            RuntimeDatabaseType resolvedType = typeArchiveJsonElement switch
            {
                ArrayArchiveJsonElement element => new RuntimeArrayType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    new RuntimeTypeRefType(element.ElementType.Id, element.ElementType.Namespace),
                    element.ElementCount
                ),
                FunctionArchiveJsonElement element => new RuntimeFunctionType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    element.Name,
                    new RuntimeTypeRefType(element.ReturnType.Id, element.ReturnType.Namespace),
                    element.Arguments.Select(x =>
                        new FunctionArgument(new RuntimeTypeRefType(x.Type.Id, x.Type.Namespace), x.Name)).ToList()
                ),
                PointerArchiveJsonElement element => new RuntimePointerType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    new RuntimeTypeRefType(element.PointedType.Id, element.PointedType.Namespace)
                ),
                PrimitiveArchiveJsonElement element => new RuntimePrimitiveType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    element.Name,
                    element.Size
                ),
                StructArchiveJsonElement element => new RuntimeStructureType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    element.Name,
                    element.Fields.Select(x => new RuntimeStructureTypeField(
                        new RuntimeTypeRefType(x.Type.Id, x.Type.Namespace),
                        x.Name
                    )).ToList()
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(typeArchiveJsonElement))
            };

            resolvedTypes[resolvedType.Id] = resolvedType;
        }

        typeArchive.Types = resolvedTypes.Values.ToList();
        typeArchive.AbsoluteFilePath = typeArchiveAbsolutePath;

        return typeArchive;
    }

    public async Task<Result> SaveTypeArchive(RuntimeTypeArchive typeArchive)
    {
        if (typeArchive.AbsoluteFilePath is null)
        {
            logger.LogError("Failed to save type-archive {ArchiveNamespace}: AbsoluteFilePath is not set", typeArchive.Namespace);
            return Result.Fail("TypeArchive.AbsoluteFilePath not set. Cannot save type-archive.");
        }

        var typeArchiveJson = new TypeArchiveJson(
            typeArchive.Namespace,
            typeArchive.Types.Select(type => type switch
            {
                RuntimeArrayType arrayType => new ArrayArchiveJsonElement
                {
                    Id = arrayType.Id,
                    ElementType = new TypeRefJsonElement()
                    {
                        Id = arrayType.ElementType.Id, Namespace = arrayType.ElementType.Namespace
                    },
                    ElementCount = arrayType.ElementCount
                },
                RuntimeFunctionType funcType => new FunctionArchiveJsonElement
                {
                    Id = funcType.Id,
                    Name = funcType.Name,
                    ReturnType =
                        new TypeRefJsonElement()
                        {
                            Id = funcType.ReturnType.Id, Namespace = funcType.ReturnType.Namespace
                        },
                    Arguments = funcType.Arguments.Select(arg => new FunctionArgumentJsonElement()
                    {
                        Type = new TypeRefJsonElement() { Id = arg.Type.Id, Namespace = arg.Type.Namespace },
                        Name = arg.Name
                    }).ToList()
                },
                RuntimePointerType ptrType => new PointerArchiveJsonElement
                {
                    Id = ptrType.Id,
                    PointedType = new TypeRefJsonElement()
                    {
                        Id = ptrType.PointedType.Id, Namespace = ptrType.PointedType.Namespace
                    }
                },
                RuntimePrimitiveType primType => new PrimitiveArchiveJsonElement
                {
                    Id = primType.Id, Name = primType.Name
                },
                RuntimeStructureType structType => new StructArchiveJsonElement
                {
                    Id = structType.Id,
                    Name = structType.Name,
                    Fields = structType.Fields.Select(field => new StructFieldArchiveJsonElement()
                    {
                        Type =
                            new TypeRefJsonElement() { Id = field.Type.Id, Namespace = field.Type.Namespace },
                        Name = field.Name
                    }).ToList()
                } as TypeArchiveJsonElement,
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            }).ToList()
        );

        await using var stream = new FileStream(typeArchive.AbsoluteFilePath, FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, typeArchiveJson,
            JsonSerializerOptionsPresets.TypeArchiveJsonOptions);

        return Result.Ok();
    }
}
