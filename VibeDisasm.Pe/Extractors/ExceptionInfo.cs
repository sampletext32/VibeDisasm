namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Contains information about exception handlers from a PE file
/// </summary>
public class ExceptionInfo
{
    /// <summary>
    /// Gets or sets the list of runtime function entries
    /// </summary>
    public List<RuntimeFunctionInfo> Functions { get; set; } = [];
}
