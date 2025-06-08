using System.IO.Compression;
using System.Text.Json;
using VibeDisasm.Web.ProjectArchive;

namespace VibeDisasm.Web.Models;

public class UserRuntimeProject
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<UserProgram> Programs { get; set; } = [];

    /// <summary>
    /// path to zip archive of the project. Can be null if the project is a newly created.
    /// </summary>
    public string? ProjectArchivePath { get; set; }

    public void Test()
    {
        using var ms = new MemoryStream();
        using var archive = new ZipArchive(ms, ZipArchiveMode.Create);

        var metadataEntry = archive.CreateEntry("metadata.json", CompressionLevel.Fastest);

        using var metadataStream = metadataEntry.Open();
        JsonSerializer.Serialize(metadataStream, new ProjectArchiveMetadata(Title, CreatedAt));
    }
}
