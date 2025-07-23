using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

/// <summary>
/// Represents a structure overlay at a specific memory address
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public record StructUserProgramDatabaseEntry(uint Address, long Size, RuntimeStructureType Type) : UserProgramDatabaseEntry(Address, Size)
{
    /// <summary>
    /// Gets the structure type associated with this entry
    /// </summary>

    private string DebugDisplay => $"struct {Type.Name} @ 0x{Address:X8}";
}
