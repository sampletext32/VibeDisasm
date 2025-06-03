using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extractor for all PE file sections
/// </summary>
public static class SectionExtractor
{
    /// <summary>
    /// Extracts all section information from a raw PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file to extract sections from</param>
    /// <returns>An array of section information</returns>
    public static List<SectionInfo> Extract(RawPeFile rawPeFile)
    {
        var sectionInfos = new List<SectionInfo>(rawPeFile.SectionHeaders.Length);

        for (var i = 0; i < rawPeFile.SectionHeaders.Length; i++)
        {
            sectionInfos.Add(BaseSectionExtractor.CreateSectionInfo(rawPeFile, rawPeFile.SectionHeaders[i]));
        }

        return sectionInfos;
    }
}
