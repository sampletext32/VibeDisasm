using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

[DebuggerDisplay("{DebugDisplay}")]
public record OverlayedArray(
    RuntimeArrayType ArrayType,
    RuntimeDatabaseType ElementType,
    List<OverlayedType> Elements,
    Memory<byte> Bytes
) : OverlayedType
{
    public override string DebugDisplay => $"overlayed {ElementType.Name}[{ArrayType.ElementCount}] [{Bytes.Length} bytes]";
};
