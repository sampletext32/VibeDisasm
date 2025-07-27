using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;
using VibeDisasm.Web.Services;

namespace VibeDisasm.Web.ProjectArchive;

public record LaterResolvedType(
    RuntimeTypeArchive Archive,
    TypeArchiveJsonElement Type
);

public partial class TypeArchiveService(
    ITypeArchiveStorage typeArchiveStorage,
    ILogger<TypeArchiveJson> logger
)
{
    public async Task LoadAllTypeArchives(
        Dictionary<ProgramTypeArchiveReference, List<RuntimeUserProgram>> programsByTypeArchivePathDict
    )
    {
        var archiveReferences = programsByTypeArchivePathDict.Keys;
        HashSet<LaterResolvedType> laterResolvedTypes = new(
            EqualityComparer<LaterResolvedType>.Create((x, y) => x!.Type.Id == y!.Type.Id, x => x.Type.Id.GetHashCode())
        );
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
                program.ReferencedTypeArchives.Add(typeArchiveLoaded.Archive);
            }

            loadedTypeArchives[typeArchiveLoaded.Archive.Namespace] = typeArchiveLoaded.Archive;

            laterResolvedTypes.AddRange(typeArchiveLoaded.Types);
        }

        // Perform resolving pass
        TypeResolvePass(laterResolvedTypes, loadedTypeArchives);
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
