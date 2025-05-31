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
    public static SectionInfo[] Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        var sectionInfos = new SectionInfo[rawPeFile.SectionHeaders.Length];

        for (var i = 0; i < rawPeFile.SectionHeaders.Length; i++)
        {
            sectionInfos[i] = BaseSectionExtractor.CreateSectionInfo(rawPeFile, rawPeFile.SectionHeaders[i]);
        }

        return sectionInfos;
    }
}
