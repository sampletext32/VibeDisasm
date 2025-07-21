using System.Diagnostics;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

[DebuggerDisplay("{InterpretedValue.DebugDisplay} ({Primitive.DebugDisplay})")]
public record OverlayedPrimitive(
    RuntimePrimitiveType Primitive,
    InterpretedValue InterpretedValue
);
