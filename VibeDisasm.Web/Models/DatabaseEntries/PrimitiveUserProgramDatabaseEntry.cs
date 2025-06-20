using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

public class PrimitiveUserProgramDatabaseEntry : UserProgramDatabaseEntry
{
    public PrimitiveUserProgramDatabaseEntry(uint address, RuntimePrimitiveType type) : base(address, type)
    {
    }
}
