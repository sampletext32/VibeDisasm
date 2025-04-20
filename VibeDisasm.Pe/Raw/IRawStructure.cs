namespace VibeDisasm.Pe.Raw;

/// <summary>
/// Interface for all raw PE structures
/// </summary>
public interface IRawStructure
{
    /// <summary>
    /// Gets the size of the structure in bytes
    /// </summary>
    int Size { get; }
}
