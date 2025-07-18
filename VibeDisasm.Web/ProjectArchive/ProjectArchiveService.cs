using System.IO.Compression;
using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.ProjectArchive;

public class ProjectArchiveService(TypeArchiveService typeArchiveService, ILogger<ProjectArchiveService> logger)
{
    public async Task<Result> Save(RuntimeUserProject runtimeProject)
    {
        logger.LogInformation(
            "Attempting to save project {ProjectId} to {ArchivePath}",
            runtimeProject.Id,
            runtimeProject.ProjectArchivePath
        );

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
                logger.LogInformation(
                    "Successfully saved project {ProjectId} to {ArchivePath}",
                    runtimeProject.Id,
                    runtimeProject.ProjectArchivePath
                );
                return Result.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to save project {ProjectId} to {ArchivePath}",
                    runtimeProject.Id,
                    runtimeProject.ProjectArchivePath
                );
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
            logger.LogError(
                ex,
                "Failed to save project {ProjectId} to {ArchivePath}",
                runtimeProject.Id,
                runtimeProject.ProjectArchivePath
            );
            return Result.Fail($"Failed to save project: {ex.Message}");
        }
    }

    private static async Task WriteProjectMetadataAsync(ZipArchive archive, RuntimeUserProject project)
    {
        var metadataEntry = archive.CreateEntry(Constants.ProjectArchive.MetadataFileName, CompressionLevel.Fastest);
        await using var metadataStream = metadataEntry.Open();
        await JsonSerializer.SerializeAsync(
            metadataStream,
            new ProjectArchiveMetadata(
                project.Id,
                project.Title,
                project.CreatedAt
            )
        );
    }

    private static async Task WriteProgramsAsync(ZipArchive archive, IEnumerable<RuntimeUserProgram> programs)
    {
        foreach (var program in programs)
        {
            var entry = archive.CreateEntry($"{Constants.ProjectArchive.ProgramsFolderName}/{program.Id}/{Constants.ProjectArchive.ProgramMetadataFileName}", CompressionLevel.Fastest);
            await using var entryStream = entry.Open();
            var archiveProgram = ProjectArchiveProgramMetadata.FromUserProgram(program);
            await JsonSerializer.SerializeAsync(entryStream, archiveProgram);
        }
    }


    public async Task<Result<RuntimeUserProject>> Load(string projectArchiveAbsolutePath)
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
                logger.LogError(
                    "Failed to deserialize project metadata from {ArchivePath}",
                    projectArchiveAbsolutePath
                );
                return Result.Fail("Failed to deserialize project metadata.");
            }

            var runtimeProject = new RuntimeUserProject
            {
                Id = jsonMetadata.ProjectId,
                Title = jsonMetadata.Title,
                CreatedAt = jsonMetadata.CreatedAt,
                ProjectArchivePath = projectArchiveAbsolutePath
            };

            var (programs, programsByTypeArchivePathDict) = await ReadProgramsAsync(archive);

            runtimeProject.Programs = programs;

            await typeArchiveService.LoadAllTypeArchive(programsByTypeArchivePathDict, runtimeProject);

            logger.LogInformation(
                "Successfully loaded project {ProjectId} from {ArchivePath}",
                jsonMetadata.ProjectId,
                projectArchiveAbsolutePath
            );
            return runtimeProject;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load project from {ArchivePath}", projectArchiveAbsolutePath);
            return Result.Fail($"Failed to load project: {ex.Message}");
        }
    }


    private static async Task<ProjectArchiveMetadata?> ReadProjectMetadataAsync(ZipArchive archive)
    {
        var metadataEntry = archive.GetEntry(Constants.ProjectArchive.MetadataFileName);
        if (metadataEntry is null) return null;
        await using var metadataStream = metadataEntry.Open();
        return await JsonSerializer.DeserializeAsync<ProjectArchiveMetadata>(metadataStream);
    }

    private static async
        Task<(List<RuntimeUserProgram> Programs, Dictionary<string, List<Guid>> ProgramsByTypeArchiveDict)>
        ReadProgramsAsync(ZipArchive archive)
    {
        var programs = new List<RuntimeUserProgram>();
        var programsByTypeArchiveDict = new Dictionary<string, List<Guid>>();

        foreach (var entry in archive.Entries)
        {
            if (entry.FullName.StartsWith(Constants.ProjectArchive.ProgramsFolderName + "/") && entry.FullName.EndsWith($"/{Constants.ProjectArchive.ProgramMetadataFileName}"))
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
