using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.Temporary;

namespace VibeDisasm.Web.Models;

/// <summary>
/// Database, holding all defined data inside the binary. Includes types and entries (function, data etc.)
/// </summary>
public class UserProgramDatabase
{
    public TypeStorage TypeStorage { get; set; }
    public DatabaseEntryManager EntryManager { get; }

    public UserProgramDatabase()
    {
        TypeStorage = new TypeStorage();
        EntryManager = new DatabaseEntryManager();

        DefaultWindowsTypesPopulator.Populate(TypeStorage);

        _ = 5;
    }
}
