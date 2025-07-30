namespace VibeDisasm.Web.Models;

/// <summary>
/// Database, holding all defined data inside the binary.
/// </summary>
public class RuntimeUserProgramDatabase
{
    public RuntimeDatabaseEntryManager EntryManager { get; }

    public RuntimeUserProgramDatabase(RuntimeUserProgram program)
    {
        EntryManager = new RuntimeDatabaseEntryManager(program);

        _ = 5;
    }
}
