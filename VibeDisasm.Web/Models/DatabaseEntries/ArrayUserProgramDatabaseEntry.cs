using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

public class ArrayUserProgramDatabaseEntry : UserProgramDatabaseEntry
{
    public ArrayUserProgramDatabaseEntry(uint address, RuntimeArrayType type) : base(address, type)
    {
    }
}
