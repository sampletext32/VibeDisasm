using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

public record PrimitiveUserProgramDatabaseEntry(uint Address, RuntimePrimitiveType Type) : UserProgramDatabaseEntry(Address, Type.Size)
{
}
