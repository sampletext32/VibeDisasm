using VibeDisasm.Web.Models.DatabaseEntries;
using VibeDisasm.Web.Temporary;

namespace VibeDisasm.Web.Models;

/// <summary>
/// Database, holding all defined data inside the binary. Includes types and entries (function, data etc.)
/// </summary>
public class UserProgramDatabase
{
    public TypeStorage TypeStorage { get; set; }
    public DatabaseEntryManager EntryManager { get; }

    public UserProgramDatabase(UserRuntimeProgram program)
    {
        TypeStorage = new TypeStorage(program);
        EntryManager = new DatabaseEntryManager(program);

        DefaultWindowsTypesPopulator.Populate(TypeStorage);

        EntryManager.AddEntry(new UndefinedUserProgramDatabaseEntry(0x0, TypeStorage.FindSemantic("undefined"), program.FileLength));

        _ = 5;
    }
}
