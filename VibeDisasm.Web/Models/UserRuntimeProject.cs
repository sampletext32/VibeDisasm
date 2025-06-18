namespace VibeDisasm.Web.Models;

public class UserRuntimeProject
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<UserRuntimeProgram> Programs { get; set; } = [];

    /// <summary>
    /// path to zip archive of the project. Can be null if the project is a newly created.
    /// </summary>
    public string? ProjectArchivePath { get; set; }
}
