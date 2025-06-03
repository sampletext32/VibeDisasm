using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Base class for section extractors with common functionality
/// </summary>
public static class BaseSectionExtractor
{
    /// <summary>
    /// Creates a section info object from a raw section header
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <param name="sectionHeader">The section header</param>
    /// <returns>A section info object</returns>
    public static SectionInfo CreateSectionInfo(RawPeFile rawPeFile, Raw.RawSectionHeader sectionHeader)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        if (sectionHeader == null)
        {
            throw new ArgumentNullException(nameof(sectionHeader));
        }

        var sectionInfo = new SectionInfo
        {
            Name = sectionHeader.Name,
            VirtualAddress = sectionHeader.VirtualAddress,
            VirtualSize = sectionHeader.VirtualSize,
            RawDataAddress = sectionHeader.PointerToRawData,
            RawDataSize = sectionHeader.SizeOfRawData,
            Characteristics = sectionHeader.Characteristics
        };

        return sectionInfo;
    }
}
