using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extractor for a specific named section
/// </summary>
public static class NamedSectionExtractor
{
    /// <summary>
    /// Extracts a specific section by name from a raw PE file
    /// </summary>
    public static SectionInfo? Extract(RawPeFile rawPeFile, string sectionName)
    {
        if (string.IsNullOrEmpty(sectionName))
        {
            throw new ArgumentException("Section name cannot be null or empty", nameof(sectionName));
        }

        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        var sectionHeader = Array.Find(rawPeFile.SectionHeaders, s => s.Name.Equals(sectionName, StringComparison.OrdinalIgnoreCase));

        if (sectionHeader == null)
        {
            return null;
        }

        return BaseSectionExtractor.CreateSectionInfo(rawPeFile, sectionHeader);
    }
}
