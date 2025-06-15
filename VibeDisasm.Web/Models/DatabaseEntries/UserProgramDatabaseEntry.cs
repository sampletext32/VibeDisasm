using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

/// <summary>
/// Entry in user-program listing, e.g. defined function, structure, array etc.
/// Must have an address of definition and type
/// </summary>
public class UserProgramDatabaseEntry
{
    public uint Address { get; set; }
    public DatabaseType Type { get; set; }

    protected UserProgramDatabaseEntry(uint address, DatabaseType type)
    {
        Address = address;
        Type = type;
    }
}
