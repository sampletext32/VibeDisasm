namespace VibeDisasm.CfgVisualizer.Models;

/// <summary>
/// Main view model that coordinates all panel view models
/// </summary>
public class MainViewModel : IViewModel
{
    // Panel view models
    public FileLoadingViewModel FileLoadingViewModel { get; }
    public EntryPointsViewModel EntryPointsViewModel { get; }
    public CfgCanvasViewModel CfgCanvasViewModel { get; }
    public NodeDetailsViewModel NodeDetailsViewModel { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public MainViewModel(FileLoadingViewModel fileLoadingViewModel, EntryPointsViewModel entryPointsViewModel, CfgCanvasViewModel cfgCanvasViewModel, NodeDetailsViewModel nodeDetailsViewModel)
    {
        FileLoadingViewModel = fileLoadingViewModel;
        EntryPointsViewModel = entryPointsViewModel;
        CfgCanvasViewModel = cfgCanvasViewModel;
        NodeDetailsViewModel = nodeDetailsViewModel;
        // Wire up events
        FileLoadingViewModel.EntryPointsLoaded += OnEntryPointsLoaded;
        FileLoadingViewModel.FunctionLoaded += OnFunctionLoaded;
        EntryPointsViewModel.EntryPointSelected += OnEntryPointSelected;
        CfgCanvasViewModel.NodeSelectionChanged += OnNodeSelectionChanged;
    }
    
    /// <summary>
    /// Called when entry points are loaded
    /// </summary>
    private void OnEntryPointsLoaded(object? sender, EntryPointsLoadedEventArgs e)
    {
        // Update the entry points view model
        EntryPointsViewModel.SetEntryPoints(e.EntryPoints);
    }
    
    /// <summary>
    /// Called when a function is loaded
    /// </summary>
    private void OnFunctionLoaded(object? sender, FunctionLoadedEventArgs e)
    {
        // Update the CFG canvas view model
        CfgCanvasViewModel.SetCfgViewModel(e.CfgViewModel);
    }
    
    /// <summary>
    /// Called when an entry point is selected
    /// </summary>
    private void OnEntryPointSelected(object? sender, EntryPointSelectedEventArgs e)
    {
        // Load the selected function
        FileLoadingViewModel.LoadFunction(e.EntryPoint.FileOffset);
    }
    
    /// <summary>
    /// Called when node selection changes
    /// </summary>
    private void OnNodeSelectionChanged(object? sender, NodeSelectionChangedEventArgs e)
    {
        // Update the node details view model
        NodeDetailsViewModel.SetSelectedNode(e.SelectedNode);
    }
}
