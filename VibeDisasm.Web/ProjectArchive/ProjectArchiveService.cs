using System.IO.Compression;
using System.Text.Json;
using FluentResults;
using Microsoft.Extensions.Logging;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.ProjectArchive;

public class ProjectArchiveService(ILogger<ProjectArchiveService> logger)
{
    public async Task<Result> Save(UserRuntimeProject runtimeProject)
    {
        logger.LogInformation("Attempting to save project {ProjectId} to {ArchivePath}", runtimeProject.Id, runtimeProject.ProjectArchivePath);

        if (runtimeProject.ProjectArchivePath is null)
        {
            logger.LogError("Failed to save project {ProjectId}: Project archive path is not set", runtimeProject.Id);
            return Result.Fail("Project archive path is not set. Cannot save project.");
        }

        try
        {
            await using var stream = new FileStream(runtimeProject.ProjectArchivePath, FileMode.OpenOrCreate);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Update, true);

            logger.LogInformation("Creating metadata entry for project {ProjectId}", runtimeProject.Id);
            var metadataEntry = archive.CreateEntry("metadata.json", CompressionLevel.Fastest);
            await using var metadataStream = metadataEntry.Open();
            await JsonSerializer.SerializeAsync(metadataStream, new ProjectArchiveMetadata(runtimeProject.Title, runtimeProject.CreatedAt));

            logger.LogInformation("Successfully saved project {ProjectId} to {ArchivePath}", runtimeProject.Id, runtimeProject.ProjectArchivePath);
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
    }

    public async Task<Result<UserRuntimeProject>> Load(string projectArchiveAbsolutePath)
    {
        logger.LogInformation("Attempting to load project from {ArchivePath}", projectArchiveAbsolutePath);

        // Check if the project archive exists
        if (!File.Exists(projectArchiveAbsolutePath))
        {
            logger.LogError("Failed to load project: File doesn't exist at {ArchivePath}", projectArchiveAbsolutePath);
            return Result.Fail("File doesn't exist");
        }

        try
        {
            await using var stream = new FileStream(projectArchiveAbsolutePath, FileMode.Open);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);

            // Get metadata entry
            logger.LogInformation("Looking for metadata entry in archive at {ArchivePath}", projectArchiveAbsolutePath);
            var metadataEntry = archive.GetEntry("metadata.json");
            if (metadataEntry is null)
            {
                logger.LogError("Invalid project archive: Metadata entry is missing in {ArchivePath}", projectArchiveAbsolutePath);
                return Result.Fail("File is not a valid project archive. Metadata entry is missing.");
            }

            // Deserialize metadata
            logger.LogInformation("Deserializing project metadata from {ArchivePath}", projectArchiveAbsolutePath);
            await using var metadataStream = metadataEntry.Open();
            var jsonMetadata = JsonSerializer.Deserialize<ProjectArchiveMetadata>(metadataStream);

            if (jsonMetadata is null)
            {
                logger.LogError("Failed to deserialize project metadata from {ArchivePath}", projectArchiveAbsolutePath);
                return Result.Fail("Failed to deserialize project metadata.");
            }

            // Create a new runtime project
            var projectId = Guid.NewGuid();
            var runtimeProject = new UserRuntimeProject {Id = projectId, Title = jsonMetadata.Title, CreatedAt = jsonMetadata.CreatedAt, ProjectArchivePath = projectArchiveAbsolutePath};

            logger.LogInformation("Successfully loaded project {ProjectId} from {ArchivePath}", projectId, projectArchiveAbsolutePath);
            return runtimeProject;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load project from {ArchivePath}", projectArchiveAbsolutePath);
            return Result.Fail($"Failed to load project: {ex.Message}");
        }
    }
}
