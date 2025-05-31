namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about Thread Local Storage (TLS) from a PE file
/// </summary>
public class TlsInfo
{
    /// <summary>
    /// Gets or sets the start address of the raw data section
    /// </summary>
    public ulong StartAddressOfRawData { get; set; }

    /// <summary>
    /// Gets or sets the end address of the raw data section
    /// </summary>
    public ulong EndAddressOfRawData { get; set; }

    /// <summary>
    /// Gets or sets the address of the index variable
    /// </summary>
    public ulong AddressOfIndex { get; set; }

    /// <summary>
    /// Gets or sets the address of the callbacks array
    /// </summary>
    public ulong AddressOfCallbacks { get; set; }

    /// <summary>
    /// Gets or sets the size of the zero fill
    /// </summary>
    public uint SizeOfZeroFill { get; set; }

    /// <summary>
    /// Gets or sets the characteristics
    /// </summary>
    public uint Characteristics { get; set; }

    /// <summary>
    /// Gets or sets the list of TLS callbacks
    /// </summary>
    public List<TlsCallbackInfo> Callbacks { get; set; } = [];
}
