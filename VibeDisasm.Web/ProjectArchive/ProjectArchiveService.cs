using System.IO.Compression;
using System.Text.Json;
using FluentResults;
using VibeDisasm.Web.Models;

namespace VibeDisasm.Web.ProjectArchive;

public class ProjectArchiveService
{
    public async Task<Result> Save(UserRuntimeProject runtimeProject)
    {
        if (runtimeProject.ProjectArchivePath is null)
        {
            return Result.Fail("Project archive path is not set. Cannot save project.");
        }

        await using var stream = new FileStream(runtimeProject.ProjectArchivePath, FileMode.OpenOrCreate);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Update, true);

        var metadataEntry = archive.CreateEntry("metadata.json", CompressionLevel.Fastest);
        await using var metadataStream = metadataEntry.Open();
        await JsonSerializer.SerializeAsync(metadataStream, new ProjectArchiveMetadata(runtimeProject.Title, runtimeProject.CreatedAt));

        return Result.Ok();
    }

    public async Task<Result<UserRuntimeProject>> Load(string projectArchiveAbsolutePath)
    {
        // Create a new project archive
        if (!File.Exists(projectArchiveAbsolutePath))
        {
            return Result.Fail("File doesn't exist");
        }

        await using var stream = new FileStream(projectArchiveAbsolutePath, FileMode.Open);
        using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);

        // Add metadata entry
        var metadataEntry = archive.GetEntry("metadata.json");
        if (metadataEntry is null)
        {
            return Result.Fail("File is not a valid project archive. Metadata entry is missing.");
        }

        await using var metadataStream = metadataEntry.Open();
        var jsonMetadata = JsonSerializer.Deserialize<ProjectArchiveMetadata>(metadataStream);

        if (jsonMetadata is null)
        {
            return Result.Fail("Failed to deserialize project metadata.");
        }

        // Create a new runtime project
        var runtimeProject = new UserRuntimeProject
        {
            Id = Guid.NewGuid(),
            Title = jsonMetadata.Title,
            CreatedAt = jsonMetadata.CreatedAt,
            ProjectArchivePath = projectArchiveAbsolutePath
        };

        return runtimeProject;
    }
}
