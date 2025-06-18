using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

public class ArrayUserProgramDatabaseEntry : UserProgramDatabaseEntry
{
    public ArrayUserProgramDatabaseEntry(uint address, ArrayType type) : base(address, type)
    {
    }
}
