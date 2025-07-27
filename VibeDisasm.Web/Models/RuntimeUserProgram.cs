namespace VibeDisasm.Web.Models;

/// <summary>
/// A runtime representation of user program.
/// </summary>
public class RuntimeUserProgram
{
    public Guid Id { get; init; }

    public string Name { get; set; }
    public string FilePath { get; init; }
    public long FileLength { get; }

    public ProgramKind Kind { get; set; }
    public ProgramArchitecture Architecture { get; set; }

    /// <summary>
    /// Types directly defined in this program
    /// </summary>
    public RuntimeTypeArchive SelfTypeArchive { get; set; }

    /// <summary>
    /// Archives of types, that this program uses
    /// </summary>
    public List<RuntimeTypeArchive> ReferencedTypeArchives { get; } = [];

    public RuntimeUserProgramDatabase Database { get; init; }

    public RuntimeUserProgram(Guid id, string filePath, string name, long fileLength)
    {
        Id = id;
        Name = name;
        FilePath = filePath;
        FileLength = fileLength;
        // self type archive is always embedded, because it is a part of the program
        SelfTypeArchive = new RuntimeTypeArchive(Name, isEmbedded: true);
        Database = new RuntimeUserProgramDatabase(this);
    }

    public int GetPointerSize() => Architecture.GetPointerSize();
}
