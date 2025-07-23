using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

[DebuggerDisplay("{DebugDisplay}")]
public record OverlayedStructure(
    RuntimeStructureType SourceStructure,
    IReadOnlyList<OverlayedStructureField> Fields,
    Memory<byte> Bytes
) : OverlayedType
{
    public OverlayedStructureField this[int index] => Fields[index];
    public OverlayedStructureField this[string name] => Fields.First(x => x.SourceField.Name == name);
    public override string DebugDisplay => $"overlayed {SourceStructure.Name} ({SourceStructure.Fields.Count} fields)";
};

[DebuggerDisplay("{DebugDisplay}")]
public record OverlayedStructureField(
    RuntimeStructureTypeField SourceField,
    OverlayedType OverlayedValue,
    Memory<byte> Bytes
) : OverlayedType
{
    public override string DebugDisplay => $"{OverlayedValue.DebugDisplay} {SourceField.Name}";
};
