using System.Diagnostics;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

[DebuggerDisplay("{Field.Name} ({InterpretedValue.Size} bytes) = {InterpretedValue.DebugDisplay}")]
public record OverlayedStructureField(
    RuntimeStructureTypeField Field,
    InterpretedValue InterpretedValue
);
