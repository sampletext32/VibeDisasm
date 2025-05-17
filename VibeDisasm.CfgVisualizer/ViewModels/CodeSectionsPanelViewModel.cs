using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.Pe.Extractors;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for the code sections panel
/// </summary>
public class CodeSectionsPanelViewModel : IViewModel
{
    /// <summary>
    /// List of section information
    /// </summary>
    public List<SectionDisplayInfo> Sections { get; private set; } = [];
    
    /// <summary>
    /// Selected section index
    /// </summary>
    public int SelectedSectionIndex { get; private set; } = -1;
    
    /// <summary>
    /// Event raised when a section is selected
    /// </summary>
    public event Action<SectionDisplayInfo>? SectionSelected;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="peFileService">Service for loading PE files</param>
    public CodeSectionsPanelViewModel(PeFileService peFileService)
    {
    }
    
    /// <summary>
    /// Sets the sections list
    /// </summary>
    /// <param name="sections">List of sections</param>
    public void SetSections(List<SectionDisplayInfo> sections)
    {
        Sections = sections;
        SelectedSectionIndex = -1;
    }
    
    /// <summary>
    /// Handles the SectionsLoaded event from the PE file service
    /// </summary>
    /// <param name="sender">Event sender</param>
    /// <param name="e">Event arguments</param>
    private void OnSectionsLoaded(object? sender, SectionsLoadedEventArgs e)
    {
        // Convert SectionInfo objects to SectionDisplayInfo objects
        var sectionDisplayInfos = e.Sections
            .Select(SectionDisplayInfo.FromSectionInfo)
            .ToList();
            
        // Update the sections list
        SetSections(sectionDisplayInfos);
    }
    
    /// <summary>
    /// Selects a section by index
    /// </summary>
    /// <param name="index">Index of the section to select</param>
    public void SelectSection(int index)
    {
        if (index >= 0 && index < Sections.Count)
        {
            SelectedSectionIndex = index;
            
            // Notify that a section has been selected
            SectionSelected?.Invoke(Sections[index]);
        }
    }
}

/// <summary>
/// Display information for a PE file section
/// </summary>
public class SectionDisplayInfo
{
    /// <summary>
    /// The name of the section
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// The virtual address of the section
    /// </summary>
    public uint VirtualAddress { get; }
    
    /// <summary>
    /// The virtual size of the section
    /// </summary>
    public uint VirtualSize { get; }
    
    /// <summary>
    /// The raw data address (file offset) of the section
    /// </summary>
    public uint RawDataAddress { get; }
    
    /// <summary>
    /// The raw data size of the section
    /// </summary>
    public uint RawDataSize { get; }
    
    /// <summary>
    /// The section characteristics
    /// </summary>
    public uint Characteristics { get; }
    
    /// <summary>
    /// Gets whether the section is executable
    /// </summary>
    public bool IsExecutable { get; }
    
    /// <summary>
    /// Gets whether the section is readable
    /// </summary>
    public bool IsReadable { get; }
    
    /// <summary>
    /// Gets whether the section is writable
    /// </summary>
    public bool IsWritable { get; }
    
    /// <summary>
    /// Gets whether the section contains code
    /// </summary>
    public bool ContainsCode { get; }
    
    /// <summary>
    /// Gets whether the section contains initialized data
    /// </summary>
    public bool ContainsInitializedData { get; }
    
    /// <summary>
    /// Gets whether the section contains uninitialized data
    /// </summary>
    public bool ContainsUninitializedData { get; }
    
    /// <summary>
    /// Computed view string for display in the UI
    /// </summary>
    public string ComputedView { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Section name</param>
    /// <param name="virtualAddress">Virtual address</param>
    /// <param name="virtualSize">Virtual size</param>
    /// <param name="rawDataAddress">Raw data address</param>
    /// <param name="rawDataSize">Raw data size</param>
    /// <param name="characteristics">Section characteristics</param>
    public SectionDisplayInfo(string name, uint virtualAddress, uint virtualSize, 
                             uint rawDataAddress, uint rawDataSize, uint characteristics)
    {
        Name = name;
        VirtualAddress = virtualAddress;
        VirtualSize = virtualSize;
        RawDataAddress = rawDataAddress;
        RawDataSize = rawDataSize;
        Characteristics = characteristics;
        
        // Calculate properties based on characteristics
        IsExecutable = (Characteristics & SectionCharacteristics.Executable) != 0;
        IsReadable = (Characteristics & SectionCharacteristics.Readable) != 0;
        IsWritable = (Characteristics & SectionCharacteristics.Writable) != 0;
        ContainsCode = (Characteristics & SectionCharacteristics.ContainsCode) != 0;
        ContainsInitializedData = (Characteristics & SectionCharacteristics.ContainsInitializedData) != 0;
        ContainsUninitializedData = (Characteristics & SectionCharacteristics.ContainsUninitializedData) != 0;
        
        // Create a computed view string
        string attributes = GetAttributesString();
        ComputedView = $"{VirtualAddress:X8} - {Name} ({VirtualSize:X} bytes) {attributes}";
    }
    
    /// <summary>
    /// Creates a section display info from a SectionInfo object
    /// </summary>
    /// <param name="sectionInfo">Source section info</param>
    /// <returns>A new SectionDisplayInfo object</returns>
    public static SectionDisplayInfo FromSectionInfo(SectionInfo sectionInfo)
    {
        return new SectionDisplayInfo(
            sectionInfo.Name,
            sectionInfo.VirtualAddress,
            sectionInfo.VirtualSize,
            sectionInfo.RawDataAddress,
            sectionInfo.RawDataSize,
            sectionInfo.Characteristics
        );
    }
    
    /// <summary>
    /// Gets a string representation of the section attributes
    /// </summary>
    /// <returns>A string with the section attributes</returns>
    private string GetAttributesString()
    {
        var attributes = new List<string>();
        
        if (IsExecutable) attributes.Add("Exec");
        if (IsReadable) attributes.Add("Read");
        if (IsWritable) attributes.Add("Write");
        if (ContainsCode) attributes.Add("Code");
        if (ContainsInitializedData) attributes.Add("Data");
        if (ContainsUninitializedData) attributes.Add("BSS");
        
        return attributes.Count > 0 ? $"[{string.Join(", ", attributes)}]" : string.Empty;
    }
}
