using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Interface for extractors that process raw PE files and extract specific data
/// </summary>
/// <typeparam name="T">The type of data to extract</typeparam>
public interface IExtractor<T>
{
    /// <summary>
    /// Extracts data from a raw PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file to extract data from</param>
    /// <returns>The extracted data</returns>
    T Extract(RawPeFile rawPeFile);
}
