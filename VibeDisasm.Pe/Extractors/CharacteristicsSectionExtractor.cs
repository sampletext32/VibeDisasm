using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extractor for sections with specific characteristics
/// </summary>
public static class CharacteristicsSectionExtractor
{
    /// <summary>
    /// Extracts sections with specific characteristics from a raw PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file to extract sections from</param>
    /// <returns>An array of matching section information</returns>
    public static SectionInfo[] Extract(RawPeFile rawPeFile, uint characteristicsMask)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        var matchingSections = Array.FindAll(rawPeFile.SectionHeaders, s => (s.Characteristics & characteristicsMask) != 0);
        var sectionInfos = new SectionInfo[matchingSections.Length];

        for (var i = 0; i < matchingSections.Length; i++)
        {
            sectionInfos[i] = BaseSectionExtractor.CreateSectionInfo(rawPeFile, matchingSections[i]);
        }

        return sectionInfos;
    }
}
