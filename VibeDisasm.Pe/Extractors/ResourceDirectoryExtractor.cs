using VibeDisasm.Pe.Models;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts resource directory information from a PE file
/// </summary>
public static class ResourceDirectoryExtractor
{
    /// <summary>
    /// Extracts resource directory information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>Resource directory information, or null if the PE file has no resource directory</returns>
    public static ResourceDirectoryInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        // Check if the PE file has a resource directory
        if (rawPeFile.ResourceDirectory == null)
        {
            return null;
        }

        // Create and populate the resource directory info
        var resourceDirectoryInfo = new ResourceDirectoryInfo
        {
            Characteristics = rawPeFile.ResourceDirectory.Characteristics,
            TimeDateStamp = rawPeFile.ResourceDirectory.TimeDateStamp,
            MajorVersion = rawPeFile.ResourceDirectory.MajorVersion,
            MinorVersion = rawPeFile.ResourceDirectory.MinorVersion,
            NumberOfNamedEntries = rawPeFile.ResourceDirectory.NumberOfNamedEntries,
            NumberOfIdEntries = rawPeFile.ResourceDirectory.NumberOfIdEntries
        };

        return resourceDirectoryInfo;
    }
}
