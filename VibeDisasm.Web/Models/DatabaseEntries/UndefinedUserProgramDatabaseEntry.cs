namespace VibeDisasm.Web.Models.DatabaseEntries;

public record UndefinedUserProgramDatabaseEntry(uint Address, long Size) : UserProgramDatabaseEntry(Address, Size);
