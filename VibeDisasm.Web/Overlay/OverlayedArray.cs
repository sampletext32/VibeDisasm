using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

[DebuggerDisplay("{DebugDisplay}")]
public record OverlayedArray(
    RuntimeArrayType ArrayType,
    List<OverlayedType> Elements,
    Memory<byte> Bytes
) : OverlayedType(ArrayType, Bytes)
{
    public OverlayedType this[int index] => Elements[index];

    public override string DebugDisplay => $"overlayed {ArrayType.Name} [{Bytes.Length} bytes]";
};
