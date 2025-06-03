namespace VibeDisasm.Web.Models;

public class UserProgram
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
    public byte[] FileData { get; set; }

    public UserProgram(Guid id, string filePath, byte[] fileData)
    {
        Id = id;
        FilePath = filePath;
        FileData = fileData;
        Name = Path.GetFileName(filePath);
    }

    public List<UserProgramFunction> Functions { get; set; } = [];
}
