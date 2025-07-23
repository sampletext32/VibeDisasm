using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

public record ArrayUserProgramDatabaseEntry(uint Address, long Size, RuntimeArrayType Type) : UserProgramDatabaseEntry(Address, Size)
{
}
