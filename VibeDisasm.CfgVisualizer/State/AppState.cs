namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// Main view model that coordinates all panel view models
/// </summary>
public class AppState
{
    // Panel view models
    public FileLoadingPanelViewModel FileLoadingPanelViewModel { get; }
    public EntryPointsPanelViewModel EntryPointsPanelViewModel { get; }
    public CfgCanvasPanelViewModel CfgCanvasPanelViewModel { get; }
    public NodeDetailsPanelViewModel NodeDetailsPanelViewModel { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public AppState(FileLoadingPanelViewModel fileLoadingPanelViewModel, EntryPointsPanelViewModel entryPointsPanelViewModel, CfgCanvasPanelViewModel cfgCanvasPanelViewModel, NodeDetailsPanelViewModel nodeDetailsPanelViewModel)
    {
        FileLoadingPanelViewModel = fileLoadingPanelViewModel;
        EntryPointsPanelViewModel = entryPointsPanelViewModel;
        CfgCanvasPanelViewModel = cfgCanvasPanelViewModel;
        NodeDetailsPanelViewModel = nodeDetailsPanelViewModel;

        // Wire up events
        FileLoadingPanelViewModel.EntryPointsLoaded += OnEntryPointsLoaded;
        FileLoadingPanelViewModel.FunctionLoaded += OnFunctionLoaded;
        EntryPointsPanelViewModel.EntryPointSelected += OnEntryPointPanelSelected;
        CfgCanvasPanelViewModel.NodeSelectionChanged += OnNodeSelectionChanged;
    }
    
    /// <summary>
    /// Called when entry points are loaded
    /// </summary>
    private void OnEntryPointsLoaded(object? sender, EntryPointsLoadedEventArgs e)
    {
        // Update the entry points view model
        EntryPointsPanelViewModel.SetEntryPoints(e.EntryPoints);
    }
    
    /// <summary>
    /// Called when a function is loaded
    /// </summary>
    private void OnFunctionLoaded(object? sender, FunctionLoadedEventArgs e)
    {
        // Update the CFG canvas view model
        CfgCanvasPanelViewModel.SetCfg(e.CfgView);
    }
    
    /// <summary>
    /// Called when an entry point is selected
    /// </summary>
    private void OnEntryPointPanelSelected(object? sender, EntryPointSelectedEventArgs e)
    {
        // Load the selected function
        FileLoadingPanelViewModel.LoadFunction(e.EntryPoint.FileOffset);
    }
    
    /// <summary>
    /// Called when node selection changes
    /// </summary>
    private void OnNodeSelectionChanged(object? sender, NodeSelectionChangedEventArgs e)
    {
        // Update the node details view model
        NodeDetailsPanelViewModel.SetSelectedNode(e.SelectedNode);
    }
}
