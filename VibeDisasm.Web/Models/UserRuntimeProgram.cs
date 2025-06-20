namespace VibeDisasm.Web.Models;

public class UserRuntimeProgram
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
    public long FileLength { get; }

    public UserProgramDatabase Database { get; set; }

    public List<RuntimeTypeArchive> TypeArchives { get; set; } = [];

    public UserRuntimeProgram(Guid id, string filePath, string name, long fileLength)
    {
        Id = id;
        Name = name;
        FilePath = filePath;
        FileLength = fileLength;
        Database = new UserProgramDatabase(this);
    }
}
