namespace VibeDisasm.Web.Models;

public class UserProgram
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }

    public UserProgram(Guid id, string filePath, string name)
    {
        Id = id;
        FilePath = filePath;
        Name = name;
    }
}
