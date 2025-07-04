using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Models.DatabaseEntries;

/// <summary>
/// Represents a structure overlay at a specific memory address
/// </summary>
[DebuggerDisplay("{DebugDisplay}")]
public class StructUserProgramDatabaseEntry : UserProgramDatabaseEntry
{
    /// <summary>
    /// Gets the structure type associated with this entry
    /// </summary>
    public RuntimeStructureType StructType => (RuntimeStructureType)Type;

    public StructUserProgramDatabaseEntry(uint address, RuntimeStructureType type) : base(address, type)
    {
    }

    private string DebugDisplay => $"struct {StructType.Name} @ 0x{Address:X8}";
}
