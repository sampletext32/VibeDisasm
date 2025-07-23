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

        var builtinArchive = DefaultWindowsTypesPopulator.CreateBuiltinArchive();
        TypeStorage.Archives.Add(builtinArchive);

        var win32Archive = DefaultWindowsTypesPopulator.CreateWin32Archive(TypeStorage);
        TypeStorage.Archives.Add(win32Archive);

        _ = 5;
    }
}
