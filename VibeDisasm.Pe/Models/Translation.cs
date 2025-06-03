namespace VibeDisasm.Pe.Models;

public class Translation
{
    /// <summary>
    /// Language ID (e.g., 0x0409 for English-US)
    /// </summary>
    public ushort LanguageId { get; set; }

    /// <summary>
    /// Codepage ID (e.g., 0x04E4 for Unicode)
    /// </summary>
    public ushort Codepage { get; set; }
}
