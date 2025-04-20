using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Extracts basic information from a PE file
/// </summary>
public class PeInfoExtractor : IExtractor<PeInfo>
{
    private readonly string _fileName;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PeInfoExtractor"/> class
    /// </summary>
    /// <param name="fileName">The name of the PE file</param>
    public PeInfoExtractor(string fileName)
    {
        _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
    }
    
    /// <summary>
    /// Extracts basic information from a PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <returns>A PeInfo object containing basic information about the PE file</returns>
    public PeInfo Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }

        return new PeInfo
        {
            FileName = _fileName,
            Is64Bit = rawPeFile.OptionalHeader.Magic == 0x20B, // PE32+ (64-bit)
            EntryPointRva = rawPeFile.OptionalHeader.AddressOfEntryPoint,
            NumberOfSections = rawPeFile.FileHeader.NumberOfSections,
            Characteristics = rawPeFile.FileHeader.Characteristics,
            Subsystem = rawPeFile.OptionalHeader.Subsystem,
            DllCharacteristics = rawPeFile.OptionalHeader.DllCharacteristics,
            SizeOfImage = rawPeFile.OptionalHeader.SizeOfImage,
            SizeOfHeaders = rawPeFile.OptionalHeader.SizeOfHeaders,
            CheckSum = rawPeFile.OptionalHeader.CheckSum
        };
    }
}
