using VibeDisasm.Pe.Raw;

namespace VibeDisasm.Pe.Extractors;

/// <summary>
/// Base class for section extractors with common functionality
/// </summary>
public abstract class BaseSectionExtractor
{
    /// <summary>
    /// Whether to include the section data in the extracted information
    /// </summary>
    public bool IncludeData { get; set; } = true;
    
    /// <summary>
    /// Creates a section info object from a raw section header
    /// </summary>
    /// <param name="rawPeFile">The raw PE file</param>
    /// <param name="sectionHeader">The section header</param>
    /// <returns>A section info object</returns>
    protected SectionInfo CreateSectionInfo(RawPeFile rawPeFile, Raw.RawSectionHeader sectionHeader)
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
        
        if (IncludeData && sectionHeader.PointerToRawData > 0 && sectionHeader.SizeOfRawData > 0)
        {
            sectionInfo.Data = rawPeFile.GetSectionData(sectionHeader);
        }
        
        return sectionInfo;
    }
}

/// <summary>
/// Extractor for all PE file sections
/// </summary>
public class SectionExtractor : BaseSectionExtractor, IExtractor<SectionInfo[]>
{
    /// <summary>
    /// Extracts all section information from a raw PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file to extract sections from</param>
    /// <returns>An array of section information</returns>
    public SectionInfo[] Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }
        
        var sectionInfos = new SectionInfo[rawPeFile.SectionHeaders.Length];
        
        for (int i = 0; i < rawPeFile.SectionHeaders.Length; i++)
        {
            sectionInfos[i] = CreateSectionInfo(rawPeFile, rawPeFile.SectionHeaders[i]);
        }
        
        return sectionInfos;
    }
}

/// <summary>
/// Extractor for a specific named section
/// </summary>
public class NamedSectionExtractor : BaseSectionExtractor, IExtractor<SectionInfo?>
{
    private readonly string _sectionName;
    
    /// <summary>
    /// Initializes a new instance of the NamedSectionExtractor class
    /// </summary>
    /// <param name="sectionName">The name of the section to extract</param>
    public NamedSectionExtractor(string sectionName)
    {
        if (string.IsNullOrEmpty(sectionName))
        {
            throw new ArgumentException("Section name cannot be null or empty", nameof(sectionName));
        }
        
        _sectionName = sectionName;
    }
    
    /// <summary>
    /// Extracts a specific section by name from a raw PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file to extract the section from</param>
    /// <returns>The section information, or null if not found</returns>
    public SectionInfo? Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }
        
        var sectionHeader = Array.Find(rawPeFile.SectionHeaders, s => s.Name.Equals(_sectionName, StringComparison.OrdinalIgnoreCase));
        
        if (sectionHeader == null)
        {
            return null;
        }
        
        return CreateSectionInfo(rawPeFile, sectionHeader);
    }
}

/// <summary>
/// Extractor for sections with specific characteristics
/// </summary>
public class CharacteristicsSectionExtractor : BaseSectionExtractor, IExtractor<SectionInfo[]>
{
    private readonly uint _characteristicsMask;
    
    /// <summary>
    /// Initializes a new instance of the CharacteristicsSectionExtractor class
    /// </summary>
    /// <param name="characteristicsMask">The characteristics mask to match</param>
    public CharacteristicsSectionExtractor(uint characteristicsMask)
    {
        _characteristicsMask = characteristicsMask;
    }
    
    /// <summary>
    /// Extracts sections with specific characteristics from a raw PE file
    /// </summary>
    /// <param name="rawPeFile">The raw PE file to extract sections from</param>
    /// <returns>An array of matching section information</returns>
    public SectionInfo[] Extract(RawPeFile rawPeFile)
    {
        if (rawPeFile == null)
        {
            throw new ArgumentNullException(nameof(rawPeFile));
        }
        
        var matchingSections = Array.FindAll(rawPeFile.SectionHeaders, s => (s.Characteristics & _characteristicsMask) != 0);
        var sectionInfos = new SectionInfo[matchingSections.Length];
        
        for (int i = 0; i < matchingSections.Length; i++)
        {
            sectionInfos[i] = CreateSectionInfo(rawPeFile, matchingSections[i]);
        }
        
        return sectionInfos;
    }
}

/// <summary>
/// Extractor for executable sections
/// </summary>
public class ExecutableSectionExtractor : CharacteristicsSectionExtractor
{
    /// <summary>
    /// Initializes a new instance of the ExecutableSectionExtractor class
    /// </summary>
    public ExecutableSectionExtractor() : base(SectionCharacteristics.Executable)
    {
    }
}

/// <summary>
/// Extractor for code sections
/// </summary>
public class CodeSectionExtractor : CharacteristicsSectionExtractor
{
    /// <summary>
    /// Initializes a new instance of the CodeSectionExtractor class
    /// </summary>
    public CodeSectionExtractor() : base(SectionCharacteristics.ContainsCode)
    {
    }
}

/// <summary>
/// Extractor for data sections
/// </summary>
public class DataSectionExtractor : CharacteristicsSectionExtractor
{
    /// <summary>
    /// Initializes a new instance of the DataSectionExtractor class
    /// </summary>
    public DataSectionExtractor() : base(SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.ContainsUninitializedData)
    {
    }
}
