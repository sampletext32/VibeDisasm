using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Temporary;

namespace VibeDisasm.Web.Models;

/// <summary>
/// Database, holding all defined data inside the binary. Includes types and entries (function, data etc.)
/// </summary>
public class RuntimeUserProgramDatabase
{
    public RuntimeTypeStorage TypeStorage { get; set; }
    public RuntimeDatabaseEntryManager EntryManager { get; }

    public RuntimeUserProgramDatabase(RuntimeUserProgram program)
    {
        TypeStorage = new RuntimeTypeStorage(program);
        EntryManager = new RuntimeDatabaseEntryManager(program);

        DefaultWindowsTypesPopulator.Populate(TypeStorage);

        // EntryManager.AddEntry(new UndefinedUserProgramDatabaseEntry(0x0, TypeStorage.FindSemantic("undefined"), program.FileLength));

        _ = 5;
    }
}
