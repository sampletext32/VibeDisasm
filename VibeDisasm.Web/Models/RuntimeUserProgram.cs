namespace VibeDisasm.Web.Models;

/// <summary>
/// A runtime representation of user program.
/// </summary>
public class RuntimeUserProgram
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
    public long FileLength { get; }

    public RuntimeUserProgramDatabase Database { get; set; }

    public List<RuntimeTypeArchive> TypeArchives { get; set; } = [];

    public RuntimeUserProgram(Guid id, string filePath, string name, long fileLength)
    {
        Id = id;
        Name = name;
        FilePath = filePath;
        FileLength = fileLength;
        Database = new RuntimeUserProgramDatabase(this);
    }
}
