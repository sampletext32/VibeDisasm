using System.Text;
using VibeDisasm.DecompilerEngine;
using VibeDisasm.DecompilerEngine.ControlFlow;
using VibeDisasm.Pe.Extractors;
using VibeDisasm.Pe.Raw;

namespace VibeDisasm.CfgVisualizer.Models;

/// <summary>
/// View model for file loading functionality
/// </summary>
public class FileLoadingViewModel : IViewModel
{
    // Loaded file path
    public string LoadedFilePath { get; private set; } = string.Empty;
    
    // File data and PE information
    private byte[] _fileData = [];
    private RawPeFile? _rawPeFile;
    private PeInfo? _peInfo;
    private ExportInfo? _exportInfo;
    
    // Code entry points
    public List<EntryPointInfo> EntryPoints { get; private set; } = [];
    
    // Events
    public event EventHandler<EntryPointsLoadedEventArgs>? EntryPointsLoaded;
    public event EventHandler<FunctionLoadedEventArgs>? FunctionLoaded;
    
    /// <summary>
    /// Tries to load a PE file and analyze its control flow
    /// </summary>
    /// <param name="filePath">Path to the PE file</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool TryLoadFile(string filePath)
    {
        try
        {
            // Clear previous data
            LoadedFilePath = string.Empty;
            _fileData = [];
            _rawPeFile = null;
            _peInfo = null;
            _exportInfo = null;
            EntryPoints = [];
            
            // Read the file bytes
            _fileData = File.ReadAllBytes(filePath);
            
            // Parse the PE file using the raw parser
            _rawPeFile = RawPeFactory.FromBytes(_fileData);
            
            // Extract basic PE information
            _peInfo = PeInfoExtractor.Extract(_rawPeFile);
            
            // Extract export information
            _exportInfo = ExportExtractor.Extract(_rawPeFile);
            
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
            
            // Store the file path and entry points
            LoadedFilePath = filePath;
            EntryPoints = entryPoints;
            
            // Raise the EntryPointsLoaded event
            EntryPointsLoaded?.Invoke(this, new EntryPointsLoadedEventArgs(EntryPoints));
            
            // If we have entry points, load the first one
            if (EntryPoints.Count > 0)
            {
                LoadFunction(EntryPoints[0].FileOffset);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading file: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Loads a function from the current PE file
    /// </summary>
    /// <param name="fileOffset">File offset of the function</param>
    public void LoadFunction(uint fileOffset)
    {
        try
        {
            // Disassemble the function into basic blocks (control flow function)
            var controlFlowFunction = ControlFlowBlockDisassembler.DisassembleBlock(_fileData, fileOffset);
            
            // Build edges
            var cfgEdges = ControlFlowEdgesBuilder.Build(controlFlowFunction);
            
            // Create the view model
            var cfgViewModel = new CfgViewModel(controlFlowFunction);
            
            // Raise the FunctionLoaded event
            FunctionLoaded?.Invoke(this, new FunctionLoadedEventArgs(cfgViewModel));
            
            // Generate Mermaid diagram for debugging
            var diagram = MermaidDiagramGenerator.GenerateDiagram(controlFlowFunction, cfgEdges);
            Console.WriteLine("Mermaid diagram:\n");
            Console.WriteLine(diagram);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading function: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Helper method to read a null-terminated string from a byte array
    /// </summary>
    /// <param name="data">Byte array</param>
    /// <param name="offset">Offset to start reading from</param>
    /// <returns>The null-terminated string</returns>
    private static string ReadNullTerminatedString(byte[] data, int offset)
    {
        int length = 0;
        while (offset + length < data.Length && data[offset + length] != 0)
        {
            length++;
        }
        
        return Encoding.ASCII.GetString(data, offset, length);
    }
}

/// <summary>
/// Information about an entry point
/// </summary>
public class EntryPointInfo
{
    /// <summary>
    /// File offset of the entry point
    /// </summary>
    public uint FileOffset { get; }
    
    /// <summary>
    /// Relative virtual address of the entry point
    /// </summary>
    public uint RVA { get; }
    
    /// <summary>
    /// Source of the entry point (e.g., "Entry Point", "Export")
    /// </summary>
    public string Source { get; }
    
    /// <summary>
    /// Description of the entry point
    /// </summary>
    public string Description { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EntryPointInfo(uint fileOffset, uint rva, string source, string description)
    {
        FileOffset = fileOffset;
        RVA = rva;
        Source = source;
        Description = description;
    }
}

/// <summary>
/// Event arguments for when entry points are loaded
/// </summary>
public class EntryPointsLoadedEventArgs : EventArgs
{
    /// <summary>
    /// List of entry points
    /// </summary>
    public List<EntryPointInfo> EntryPoints { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EntryPointsLoadedEventArgs(List<EntryPointInfo> entryPoints)
    {
        EntryPoints = entryPoints;
    }
}

/// <summary>
/// Event arguments for when a function is loaded
/// </summary>
public class FunctionLoadedEventArgs : EventArgs
{
    /// <summary>
    /// CFG view model for the loaded function
    /// </summary>
    public CfgViewModel CfgViewModel { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public FunctionLoadedEventArgs(CfgViewModel cfgViewModel)
    {
        CfgViewModel = cfgViewModel;
    }
}
