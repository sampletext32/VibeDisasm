namespace VibeDisasm.Web.Models;

public class UserProgramFunction
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public UserProgramFunction(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}
