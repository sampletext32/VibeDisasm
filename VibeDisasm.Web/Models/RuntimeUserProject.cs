namespace VibeDisasm.Web.Models;

/// <summary>
/// A runtime representation of user project (basically a pack of programs)
/// </summary>
public class RuntimeUserProject
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<RuntimeUserProgram> Programs { get; set; } = [];

    /// <summary>
    /// path to zip archive of the project. Can be null if the project is a newly created.
    /// </summary>
    public string? ProjectArchivePath { get; set; }

    public RuntimeUserProgram? GetProgram(Guid programId)
    {
        return Programs.FirstOrDefault(p => p.Id == programId);
    }
}
