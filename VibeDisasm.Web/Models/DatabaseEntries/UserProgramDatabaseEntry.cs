using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

/// <summary>
/// Entry in user-program listing, e.g. defined function, structure, array etc.
/// Must have an address of definition and type
/// </summary>
public abstract class UserProgramDatabaseEntry
{
    public uint Address { get; set; }
    public RuntimeDatabaseType Type { get; set; }

    protected UserProgramDatabaseEntry(uint address, RuntimeDatabaseType type)
    {
        Address = address;
        Type = type;
    }
}
