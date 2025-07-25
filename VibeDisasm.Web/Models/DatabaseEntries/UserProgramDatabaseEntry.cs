namespace VibeDisasm.Web.Models.DatabaseEntries;

/// <summary>
/// Entry in user-program listing, e.g. defined function, structure, array etc.
/// </summary>
public abstract record UserProgramDatabaseEntry(uint Address, long Size);
