using System.IO.Compression;
using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

namespace VibeDisasm.Web.ProjectArchive;

public class ProjectArchiveService(ILogger<ProjectArchiveService> logger)
{
    public async Task<Result> Save(UserRuntimeProject runtimeProject)
    {
        logger.LogInformation("Attempting to save project {ProjectId} to {ArchivePath}", runtimeProject.Id,
            runtimeProject.ProjectArchivePath);

        if (runtimeProject.ProjectArchivePath is null)
        {
            logger.LogError("Failed to save project {ProjectId}: Project archive path is not set", runtimeProject.Id);
            return Result.Fail("Project archive path is not set. Cannot save project.");
        }

        try
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.tmp");
            try
            {
                await using (var stream =
                             new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    await WriteProjectMetadataAsync(archive, runtimeProject);
                    await WriteProgramsAsync(archive, runtimeProject.Programs);
                }

                File.Copy(tempFile, runtimeProject.ProjectArchivePath, overwrite: true);
                logger.LogInformation("Successfully saved project {ProjectId} to {ArchivePath}", runtimeProject.Id,
                    runtimeProject.ProjectArchivePath);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save project {ProjectId} to {ArchivePath}", runtimeProject.Id,
                    runtimeProject.ProjectArchivePath);
                return Result.Fail($"Failed to save project: {ex.Message}");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save project {ProjectId} to {ArchivePath}", runtimeProject.Id,
                runtimeProject.ProjectArchivePath);
            return Result.Fail($"Failed to save project: {ex.Message}");
        }
    }

    private static async Task WriteProjectMetadataAsync(ZipArchive archive, UserRuntimeProject project)
    {
        var metadataEntry = archive.CreateEntry("metadata.json", CompressionLevel.Fastest);
        await using var metadataStream = metadataEntry.Open();
        await JsonSerializer.SerializeAsync(metadataStream,
            new ProjectArchiveMetadata(
                project.Id,
                project.Title,
                project.CreatedAt
            ));
    }

    private static async Task WriteProgramsAsync(ZipArchive archive, IEnumerable<UserRuntimeProgram> programs)
    {
        foreach (var program in programs)
        {
            var entry = archive.CreateEntry($"programs/{program.Id}/metadata.json", CompressionLevel.Fastest);
            await using var entryStream = entry.Open();
            var archiveProgram = ProjectArchiveProgramMetadata.FromUserProgram(program);
            await JsonSerializer.SerializeAsync(entryStream, archiveProgram);
        }
    }


    public async Task<Result<UserRuntimeProject>> Load(string projectArchiveAbsolutePath)
    {
        logger.LogInformation("Attempting to load project from {ArchivePath}", projectArchiveAbsolutePath);

        if (!File.Exists(projectArchiveAbsolutePath))
        {
            logger.LogError("Failed to load project: File doesn't exist at {ArchivePath}", projectArchiveAbsolutePath);
            return Result.Fail("File doesn't exist");
        }

        try
        {
            await using var stream = new FileStream(projectArchiveAbsolutePath, FileMode.Open);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);

            var jsonMetadata = await ReadProjectMetadataAsync(archive);
            if (jsonMetadata is null)
            {
                logger.LogError("Failed to deserialize project metadata from {ArchivePath}",
                    projectArchiveAbsolutePath);
                return Result.Fail("Failed to deserialize project metadata.");
            }

            var runtimeProject = new UserRuntimeProject
            {
                Id = jsonMetadata.ProjectId,
                Title = jsonMetadata.Title,
                CreatedAt = jsonMetadata.CreatedAt,
                ProjectArchivePath = projectArchiveAbsolutePath
            };

            var (programs, programsByTypeArchivePathDict) = await ReadProgramsAsync(archive);

            runtimeProject.Programs = programs;

            var distinctTypeArchivePaths = programsByTypeArchivePathDict.Keys;

            foreach (var typeArchivePath in distinctTypeArchivePaths)
            {
                var typeArchive = await LoadTypeArchive(typeArchivePath);

                if (typeArchive is null)
                {
                    logger.LogWarning("Failed to load type archive for {TypeArchivePath}. Program may not work.",
                        typeArchivePath);
                    continue;
                }

                foreach (var programId in programsByTypeArchivePathDict[typeArchivePath])
                {
                    var program = runtimeProject.Programs.FirstOrDefault(x => x.Id == programId) ??
                                  throw new InvalidOperationException($"Failed to find program with id {programId}");

                    program.TypeArchives.Add(typeArchive);
                }
            }

            logger.LogInformation("Successfully loaded project {ProjectId} from {ArchivePath}", jsonMetadata.ProjectId,
                projectArchiveAbsolutePath);
            return runtimeProject;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load project from {ArchivePath}", projectArchiveAbsolutePath);
            return Result.Fail($"Failed to load project: {ex.Message}");
        }
    }

    private async Task<RuntimeTypeArchive?> LoadTypeArchive(string typeArchivePath)
    {
        if (Path.IsPathFullyQualified(typeArchivePath))
        {
            logger.LogInformation("Attempting to load type archive from absolute path {TypeArchivePath}",
                typeArchivePath);
        }
        else
        {
            logger.LogInformation("Attempting to load type archive from relative path {TypeArchivePath}",
                typeArchivePath);
        }

        if (!File.Exists(typeArchivePath))
        {
            return null;
        }

        await using var stream = new FileStream(typeArchivePath, FileMode.Open);
        var typeArchiveJson =
            await JsonSerializer.DeserializeAsync<TypeArchiveJson>(stream,
                JsonSerializerOptionsPresets.TypeArchiveJsonOptions);

        if (typeArchiveJson is null)
        {
            logger.LogWarning("TypeArchive is corrupted. Deserialization Failed");
            return null;
        }

        var typeArchive = new RuntimeTypeArchive(typeArchiveJson.Namespace);

        Dictionary<Guid, DatabaseType> resolvedTypes = [];

        foreach (var typeArchiveJsonElement in typeArchiveJson.Types)
        {
            DatabaseType resolvedType = typeArchiveJsonElement switch
            {
                ArrayArchiveJsonElement element => new ArrayType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    resolvedTypes[element.ElementTypeId],
                    element.ElementCount
                ),
                FunctionArchiveJsonElement element => new FunctionType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    element.Name,
                    resolvedTypes[element.ReturnTypeId],
                    element.Arguments.Select(x => new FunctionArgument(resolvedTypes[x.TypeId], x.Name)).ToList()
                ),
                PointerArchiveJsonElement element => new PointerType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    element.Name,
                    resolvedTypes[element.PointedTypeId]
                ),
                PrimitiveArchiveJsonElement element => new PrimitiveType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    element.Name
                ),
                StructArchiveJsonElement element => new StructureType(
                    element.Id,
                    typeArchiveJson.Namespace,
                    element.Name,
                    element.Fields.Select(x => new StructureTypeField(
                        resolvedTypes[x.TypeId],
                        x.Name
                    )).ToList()
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(typeArchiveJsonElement))
            };

            resolvedTypes[resolvedType.Id] = resolvedType;
        }

        typeArchive.Types = resolvedTypes.Values.ToList();

        return typeArchive;
    }

    private static async Task<ProjectArchiveMetadata?> ReadProjectMetadataAsync(ZipArchive archive)
    {
        var metadataEntry = archive.GetEntry("metadata.json");
        if (metadataEntry is null) return null;
        await using var metadataStream = metadataEntry.Open();
        return await JsonSerializer.DeserializeAsync<ProjectArchiveMetadata>(metadataStream);
    }

    private static async
        Task<(List<UserRuntimeProgram> Programs, Dictionary<string, List<Guid>> ProgramsByTypeArchiveDict)>
        ReadProgramsAsync(ZipArchive archive)
    {
        var programs = new List<UserRuntimeProgram>();
        var programsByTypeArchiveDict = new Dictionary<string, List<Guid>>();

        foreach (var entry in archive.Entries)
        {
            if (entry.FullName.StartsWith("programs/") && entry.FullName.EndsWith("/metadata.json"))
            {
                await using var programStream = entry.Open();
                var archiveProgram =
                    await JsonSerializer.DeserializeAsync<ProjectArchiveProgramMetadata>(programStream);

                if (archiveProgram is not null)
                {
                    var userRuntimeProgram = archiveProgram.ToUserProgram();
                    programs.Add(userRuntimeProgram);

                    foreach (var programTypeArchivePath in archiveProgram.TypeArchivePaths)
                    {
                        if (programsByTypeArchiveDict.TryGetValue(programTypeArchivePath, out var list))
                        {
                            list.Add(userRuntimeProgram.Id);
                        }
                        else
                        {
                            programsByTypeArchiveDict[programTypeArchivePath] = [userRuntimeProgram.Id];
                        }
                    }
                }
            }
        }

        return (programs, programsByTypeArchiveDict);
    }
}
