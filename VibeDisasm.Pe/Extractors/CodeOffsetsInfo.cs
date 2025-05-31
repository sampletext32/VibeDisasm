namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about all definite code offsets in a PE file
/// </summary>
public class CodeOffsetsInfo
{
    /// <summary>
    /// Gets or sets the list of code offsets
    /// </summary>
    public List<CodeOffsetInfo> Offsets { get; set; } = [];
}
