using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

[DebuggerDisplay("overlayed {Structure.Name} ({Structure.Fields.Count} fields)")]
public record OverlayedStructure(RuntimeStructureType Structure, IReadOnlyList<OverlayedStructureField> Fields, int ByteSize)
{
    public int ByteSize { get; } = ByteSize;

    public OverlayedStructureField this[int index] => Fields[index];
    public OverlayedStructureField this[string name] => Fields.First(x => x.Field.Name == name);
};
