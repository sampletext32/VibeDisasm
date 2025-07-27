using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.ProjectArchive;

public record LaterResolvedType(
    RuntimeTypeArchive Archive,
    TypeArchiveJsonElement Type
);

public class TypeArchiveService(
    ITypeArchiveStorage typeArchiveStorage,
    ILogger<TypeArchiveJson> logger
)
{
    public async Task LoadAllTypeArchives(
        Dictionary<ProgramTypeArchiveReference, List<RuntimeUserProgram>> programsByTypeArchivePathDict,
        RuntimeUserProject runtimeProject
    )
    {
        var archiveReferences = programsByTypeArchivePathDict.Keys;
        Queue<LaterResolvedType> queue = new();
        Dictionary<string, RuntimeTypeArchive> loadedTypeArchives = new();

        foreach (var archiveReference in archiveReferences)
        {
            if (archiveReference.IsEmbedded)
            {
                if (typeArchiveStorage.TypeArchives.All(x => x.Namespace != archiveReference.PathOrNamespace))
                {
                    logger.LogWarning(
                        "Failed to locate embedded type archive for namespace {Namespace}. Program may not work.",
                        archiveReference.PathOrNamespace
                    );
                }

                continue;
            }

            var archivePath = archiveReference.PathOrNamespace;
            if (Path.IsPathFullyQualified(archivePath))
            {
                logger.LogInformation(
                    "Attempting to load type archive from absolute path {TypeArchivePath}",
                    archivePath
                );
            }
            else
            {
                archivePath = Path.GetFullPath(archivePath);
                logger.LogInformation(
                    "Attempting to load type archive from relative path {archivePath}",
                    archivePath
                );
            }

            var typeArchiveLoaded = await LoadTypeArchive(archivePath);

            if (typeArchiveLoaded.Archive is null)
            {
                logger.LogWarning(
                    "Failed to load type archive for {TypeArchivePath}. Program may not work.",
                    archivePath
                );
                continue;
            }

            foreach (var program in programsByTypeArchivePathDict[archiveReference])
            {
                program.TypeArchives.Add(typeArchiveLoaded.Archive);
            }

            loadedTypeArchives[typeArchiveLoaded.Archive.Namespace] = typeArchiveLoaded.Archive;

            queue.EnqueueRange(typeArchiveLoaded.Types);
        }

        // Perform resolving pass
        TypeResolvePass(queue, loadedTypeArchives);
    }

    private void TypeResolvePass(
        Queue<LaterResolvedType> queue,
        Dictionary<string, RuntimeTypeArchive> loadedTypeArchives
    )
    {
        var prevCount = -1;
        while (queue.Count > 0)
        {
            if (queue.Count == prevCount)
            {
                logger.LogWarning(
                    "Type resolve pass is stuck, queue size: {QueueSize}. Types:{Types}",
                    queue.Count,
                    string.Join(", ", queue.Select(x => x.Type.Id))
                );

                throw new InvalidOperationException(
                    "Type resolve pass is stuck, queue size did not change."
                );
            }

            var resolveType = queue.Dequeue();

            RuntimeDatabaseType resolvedType;
            switch (resolveType.Type)
            {
                case ArrayArchiveJsonElement element:
                {
                    var resolvedElementType = FindType(element.ElementType, loadedTypeArchives);

                    if (resolvedElementType is null)
                    {
                        queue.Enqueue(resolveType);
                        break;
                    }

                    resolvedType = new RuntimeArrayType(
                        element.Id,
                        resolveType.Archive.Namespace,
                        resolvedElementType,
                        element.ElementCount
                    );
                    resolveType.Archive.AddType(resolvedType);
                    break;
                }
                case FunctionArchiveJsonElement element:
                {
                    var resolvedReturnType = FindType(element.ReturnType, loadedTypeArchives);

                    if (resolvedReturnType is null)
                    {
                        queue.Enqueue(resolveType);
                        break;
                    }

                    Dictionary<string, RuntimeDatabaseType> argumentsByName = new();

                    var hasUnresolvedArgument = false;
                    foreach (var functionArgumentJsonElement in element.Arguments)
                    {
                        var argumentType = FindType(functionArgumentJsonElement.Type, loadedTypeArchives);

                        if (argumentType is null)
                        {
                            hasUnresolvedArgument = true;
                            break;
                        }

                        argumentsByName[functionArgumentJsonElement.Name] = argumentType;
                    }

                    if (hasUnresolvedArgument)
                    {
                        queue.Enqueue(resolveType);
                        break;
                    }

                    resolvedType = new RuntimeFunctionType(
                        element.Id,
                        resolveType.Archive.Namespace,
                        element.Name,
                        resolvedReturnType,
                        element.Arguments.Select(x => new FunctionArgument(
                                    argumentsByName[x.Name],
                                    x.Name
                                )
                            )
                            .ToList()
                    );
                    resolveType.Archive.AddType(resolvedType);
                    break;
                }
                case PointerArchiveJsonElement element:
                {
                    var resolvedPointedType = FindType(element.PointedType, loadedTypeArchives);

                    if (resolvedPointedType is null)
                    {
                        queue.Enqueue(resolveType);
                        break;
                    }

                    resolvedType = new RuntimePointerType(
                        element.Id,
                        resolveType.Archive.Namespace,
                        resolvedPointedType
                    );
                    resolveType.Archive.AddType(resolvedType);
                    break;
                }
                case PrimitiveArchiveJsonElement element:
                {
                    resolvedType = new RuntimePrimitiveType(
                        element.Id,
                        resolveType.Archive.Namespace,
                        element.Name,
                        element.Size
                    );
                    resolveType.Archive.AddType(resolvedType);
                    break;
                }
                case StructArchiveJsonElement element:
                {
                    Dictionary<string, RuntimeDatabaseType> fieldsByName = new();

                    var hasUnresolvedField = false;
                    foreach (var structFieldArchiveJsonElement in element.Fields)
                    {
                        var resolvedFieldType = FindType(structFieldArchiveJsonElement.Type, loadedTypeArchives);

                        if (resolvedFieldType is null)
                        {
                            hasUnresolvedField = true;
                            break;
                        }

                        fieldsByName[structFieldArchiveJsonElement.Name] = resolvedFieldType;
                    }

                    if (hasUnresolvedField)
                    {
                        // TODO: recursive structures will fail to resolve (linked list for example). Fix ASAP
                        queue.Enqueue(resolveType);
                        break;
                    }

                    resolvedType = new RuntimeStructureType(
                        element.Id,
                        resolveType.Archive.Namespace,
                        element.Name,
                        element.Fields.Select(x => new RuntimeStructureTypeField(
                                    fieldsByName[x.Name],
                                    x.Name
                                )
                            )
                            .ToList()
                    );
                    resolveType.Archive.AddType(resolvedType);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException("Unknown type of resolveType", (Exception?)null);
                }
            }
        }
    }

    private RuntimeDatabaseType? FindType(
        TypeRefJsonElement reference,
        Dictionary<string, RuntimeTypeArchive> loadedTypeArchives
    )
    {
        // first try to find in embedded types, then in loaded type archives

        var embeddedArchive =
            typeArchiveStorage.TypeArchives.FirstOrDefault(x => x.Namespace == reference.Namespace);

        var foundType = embeddedArchive?.FindById(reference.Id);

        if (foundType != null)
        {
            return foundType;
        }

        if (loadedTypeArchives.TryGetValue(reference.Namespace, out var archive))
        {
            foundType = archive.FindById(reference.Id);
            if (foundType != null)
            {
                return foundType;
            }
        }

        logger.LogWarning(
            "Failed to resolve type {TypeId} in namespace {Namespace}. Type may not work.",
            reference.Id,
            reference.Namespace
        );

        return null;
    }

    public async Task<(RuntimeTypeArchive? Archive, List<LaterResolvedType> Types)> LoadTypeArchive(
        string typeArchiveAbsolutePath
    )
    {
        if (!File.Exists(typeArchiveAbsolutePath))
        {
            logger.LogWarning("TypeArchive at {ArchivePath} does not exist", typeArchiveAbsolutePath);
            return (null, []);
        }

        await using var stream = new FileStream(typeArchiveAbsolutePath, FileMode.Open);
        var typeArchiveJson =
            await JsonSerializer.DeserializeAsync<TypeArchiveJson>(
                stream,
                JsonSerializerOptionsPresets.TypeArchiveJsonOptions
            );

        if (typeArchiveJson is null)
        {
            logger.LogWarning(
                "TypeArchive at {ArchivePath} is corrupted. Deserialization Failed",
                typeArchiveAbsolutePath
            );
            return (null, []);
        }

        // archives from the file system are not embedded by design
        var typeArchive = new RuntimeTypeArchive(typeArchiveJson.Namespace, isEmbedded: false);

        typeArchive.AbsoluteFilePath = typeArchiveAbsolutePath;
        return (typeArchive, typeArchiveJson.Types.Select(x => new LaterResolvedType(typeArchive, x)).ToList());

        // var typeQueue = new Queue<TypeArchiveJsonElement>(typeArchiveJson.Types);
        //
        // Dictionary<Guid, RuntimeDatabaseType> resolvedTypes = [];
        //
        // foreach (var typeArchiveJsonElement in typeArchiveJson.Types)
        // {
        //     RuntimeDatabaseType resolvedType = typeArchiveJsonElement switch
        //     {
        //         ArrayArchiveJsonElement element => new RuntimeArrayType(
        //             element.Id,
        //             typeArchiveJson.Namespace,
        //             new RuntimeTypeRefType(element.ElementType.Id, element.ElementType.Namespace),
        //             element.ElementCount
        //         ),
        //         FunctionArchiveJsonElement element => new RuntimeFunctionType(
        //             element.Id,
        //             typeArchiveJson.Namespace,
        //             element.Name,
        //             new RuntimeTypeRefType(element.ReturnType.Id, element.ReturnType.Namespace),
        //             element.Arguments.Select(x =>
        //                     new FunctionArgument(new RuntimeTypeRefType(x.Type.Id, x.Type.Namespace), x.Name)
        //                 )
        //                 .ToList()
        //         ),
        //         PointerArchiveJsonElement element => new RuntimePointerType(
        //             element.Id,
        //             typeArchiveJson.Namespace,
        //             new RuntimeTypeRefType(element.PointedType.Id, element.PointedType.Namespace)
        //         ),
        //         PrimitiveArchiveJsonElement element => new RuntimePrimitiveType(
        //             element.Id,
        //             typeArchiveJson.Namespace,
        //             element.Name,
        //             element.Size
        //         ),
        //         StructArchiveJsonElement element => new RuntimeStructureType(
        //             element.Id,
        //             typeArchiveJson.Namespace,
        //             element.Name,
        //             element.Fields.Select(x => new RuntimeStructureTypeField(
        //                         new RuntimeTypeRefType(x.Type.Id, x.Type.Namespace),
        //                         x.Name
        //                     )
        //                 )
        //                 .ToList()
        //         ),
        //         _ => throw new ArgumentOutOfRangeException(nameof(typeArchiveJsonElement))
        //     };
        //
        //     resolvedTypes[resolvedType.Id] = resolvedType;
        // }
        //
        // typeArchive.Types = resolvedTypes.Values.ToList();
        // typeArchive.AbsoluteFilePath = typeArchiveAbsolutePath;
        //
        // return typeArchive;
    }

    public async Task<Result> SaveTypeArchive(RuntimeTypeArchive typeArchive)
    {
        if (typeArchive.IsEmbedded)
        {
            logger.LogError(
                "Failed to save type-archive {ArchiveNamespace}: Archive is embedded and cannot be saved",
                typeArchive.Namespace
            );
            return Result.Fail("TypeArchive is embedded and cannot be saved to a file.");
        }

        if (typeArchive.AbsoluteFilePath is null)
        {
            logger.LogError(
                "Failed to save type-archive {ArchiveNamespace}: AbsoluteFilePath is not set",
                typeArchive.Namespace
            );
            return Result.Fail("TypeArchive.AbsoluteFilePath not set. Cannot save type-archive.");
        }

        var visitor = new TypeArchiveToJsonVisitor();

        var typeArchiveJson = new TypeArchiveJson(
            typeArchive.Namespace,
            typeArchive.Types.Select(type => visitor.Visit(type)).ToList()
        );

        await using var stream = new FileStream(typeArchive.AbsoluteFilePath, FileMode.Create);
        await JsonSerializer.SerializeAsync(
            stream,
            typeArchiveJson,
            JsonSerializerOptionsPresets.TypeArchiveJsonOptions
        );

        return Result.Ok();
    }
}
