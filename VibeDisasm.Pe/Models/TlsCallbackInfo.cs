namespace VibeDisasm.Pe.Models;

/// <summary>
/// Contains information about a TLS callback function in a PE file
/// </summary>
public class TlsCallbackInfo
{
    /// <summary>
    /// Gets or sets the index of the callback in the array
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the relative virtual address (RVA) of the callback
    /// </summary>
    public uint RelativeVirtualAddress { get; set; }

    /// <summary>
    /// Gets or sets the file offset of the callback
    /// </summary>
    public uint FileOffset { get; set; }
}
