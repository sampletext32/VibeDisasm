using System.Diagnostics;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.CfgVisualizer.Services;

/// <summary>
/// Service for loading and analyzing PE files
/// </summary>
public class PeFileService : IService
{
    // File data and PE information
    private byte[]? _fileData;
    private RawPeFile? _rawPeFile;
    private PeInfo? _peInfo;
    private ExportInfo? _exportInfo;
    private SectionInfo[]? _sections;
    
    /// <summary>
    /// Currently loaded file path
    /// </summary>
    public string LoadedFilePath { get; private set; } = string.Empty;
    
    /// <summary>
    /// Entry points found in the PE file
    /// </summary>
    public List<EntryPointInfo> EntryPoints { get; private set; } = [];
    
    /// <summary>
    /// Sections found in the PE file
    /// </summary>
    public List<SectionInfo> Sections { get; private set; } = [];
    
    /// <summary>
    /// Event raised when a PE file is loaded
    /// </summary>
    public event Action<PeFileLoadedEventArgs>? PeFileLoaded;
    
    /// <summary>
    /// Tries to load a PE file and analyze its structure
    /// </summary>
    /// <param name="filePath">Path to the PE file</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool TryLoadFile(string filePath)
    {
        try
        {
            // Clear previous data
            ClearData();
            
            // Read the file bytes
            _fileData = File.ReadAllBytes(filePath);
            
            // Parse the PE file using the raw parser
            _rawPeFile = RawPeFactory.FromBytes(_fileData);
            
            // Extract basic PE information
            _peInfo = PeInfoExtractor.Extract(_rawPeFile);
            
            // Extract export information
            _exportInfo = ExportExtractor.Extract(_rawPeFile);
            
            // Extract all sections
            _sections = SectionExtractor.Extract(_rawPeFile);
            
            // Store the file path
            LoadedFilePath = filePath;
            
            // Extract entry points
            ExtractEntryPoints();
            
            // Raise the PeFileLoaded event
            PeFileLoaded?.Invoke(new PeFileLoadedEventArgs(LoadedFilePath, _peInfo));
            
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading file: {ex.Message}");
            ClearData();
            return false;
        }
    }
    
    /// <summary>
    /// Extracts entry points from the loaded PE file
    /// </summary>
    private void ExtractEntryPoints()
    {
        if (_rawPeFile is null || _peInfo is null)
        {
            return;
        }
        
        // Create a list to store all definite code offsets
        var entryPoints = new List<EntryPointInfo>();
        
        // 1. Add entry point as definite code
        if (_peInfo.EntryPointRva > 0)
        {
            uint entryPointOffset = Util.RvaToOffset(_rawPeFile, _peInfo.EntryPointRva);
            entryPoints.Add(new EntryPointInfo(
                entryPointOffset,
                _peInfo.EntryPointRva,
                "Entry Point",
                "Program entry point"));
        }
        
        // 2. Add exported functions as definite code
        if (_exportInfo != null && _exportInfo.Functions.Count > 0)
        {
            foreach (var exportedFunction in _exportInfo.Functions)
            {
                // Skip forwarded exports (they don't have code in this file)
                if (exportedFunction.IsForwarded)
                    continue;
                    
                uint exportOffset = Util.RvaToOffset(_rawPeFile, exportedFunction.RelativeVirtualAddress);
                entryPoints.Add(new EntryPointInfo(
                    exportOffset,
                    exportedFunction.RelativeVirtualAddress,
                    "Export",
                    $"Exported function: {exportedFunction.Name} (Ordinal: {exportedFunction.Ordinal})"));
            }
        }
        
        // Store the entry points
        EntryPoints = entryPoints;
    }
    
    /// <summary>
    /// Clears all loaded data
    /// </summary>
    private void ClearData()
    {
        LoadedFilePath = string.Empty;
        _fileData = null;
        _rawPeFile = null;
        _peInfo = null;
        _exportInfo = null;
        _sections = null;
        EntryPoints = [];
        Sections = [];
    }
}

/// <summary>
/// Event arguments for when a PE file is loaded
/// </summary>
public class PeFileLoadedEventArgs : EventArgs
{
    /// <summary>
    /// Path to the loaded PE file
    /// </summary>
    public string FilePath { get; }
    
    /// <summary>
    /// Basic PE information
    /// </summary>
    public PeInfo PeInfo { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="filePath">Path to the loaded PE file</param>
    /// <param name="peInfo">Basic PE information</param>
    public PeFileLoadedEventArgs(string filePath, PeInfo peInfo)
    {
        FilePath = filePath;
        PeInfo = peInfo;
    }
}

/// <summary>
/// Event arguments for when sections are loaded
/// </summary>
public class SectionsLoadedEventArgs : EventArgs
{
    /// <summary>
    /// List of sections
    /// </summary>
    public List<SectionInfo> Sections { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sections">List of sections</param>
    public SectionsLoadedEventArgs(List<SectionInfo> sections)
    {
        Sections = sections;
    }
}
