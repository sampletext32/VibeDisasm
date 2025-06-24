namespace VibeDisasm.Web.Models;

/// <summary>
/// A runtime representation of user program.
/// </summary>
public class RuntimeUserProgram
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string FilePath { get; init; }
    public long FileLength { get; }

    public RuntimeUserProgramDatabase Database { get; init; }

    public RuntimeUserProgram(Guid id, string filePath, string name, long fileLength)
    {
        Id = id;
        Name = name;
        FilePath = filePath;
        FileLength = fileLength;
        Database = new RuntimeUserProgramDatabase(this);
    }
}
