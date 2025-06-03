namespace VibeDisasm.Web.Models;

public class UserProject
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<UserProgram> Programs { get; set; } = [];
}
