using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Overlay;

public abstract record OverlayedType(
    IRuntimeDatabaseType UnderlyingType,
    Memory<byte> Bytes
)
{
    public abstract string DebugDisplay { get; }
}
