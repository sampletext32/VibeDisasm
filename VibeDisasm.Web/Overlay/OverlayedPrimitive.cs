using System.Diagnostics;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

[DebuggerDisplay("{DebugDisplay}")]
public record OverlayedPrimitive(
    RuntimePrimitiveType Primitive,
    Memory<byte> Bytes
) : OverlayedType(Primitive, Bytes)
{
    public override string DebugDisplay => $"overlayed {Primitive.DebugDisplay} [{Bytes.Length} bytes]";
};
