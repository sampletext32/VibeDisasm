namespace VibeDisasm.CfgVisualizer.Models;

/// <summary>
/// View model for the entry points panel
/// </summary>
public class EntryPointsViewModel : IViewModel
{
    // Entry points list
    public List<EntryPointInfo> EntryPoints { get; private set; } = [];
    
    // Selected entry point index
    public int SelectedEntryPointIndex { get; private set; } = -1;
    
    // Events
    public event EventHandler<EntryPointSelectedEventArgs>? EntryPointSelected;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EntryPointsViewModel()
    {
    }
    
    /// <summary>
    /// Sets the entry points list
    /// </summary>
    /// <param name="entryPoints">Entry points list</param>
    public void SetEntryPoints(List<EntryPointInfo> entryPoints)
    {
        EntryPoints = entryPoints;
        SelectedEntryPointIndex = -1;
    }
    
    /// <summary>
    /// Selects an entry point by index
    /// </summary>
    /// <param name="index">Index of the entry point to select</param>
    public void SelectEntryPoint(int index)
    {
        if (index >= 0 && index < EntryPoints.Count)
        {
            SelectedEntryPointIndex = index;
            
            // Notify that an entry point has been selected
            EntryPointSelected?.Invoke(this, new EntryPointSelectedEventArgs(EntryPoints[index]));
        }
    }
}

/// <summary>
/// Event arguments for when an entry point is selected
/// </summary>
public class EntryPointSelectedEventArgs : EventArgs
{
    /// <summary>
    /// Selected entry point
    /// </summary>
    public EntryPointInfo EntryPoint { get; }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EntryPointSelectedEventArgs(EntryPointInfo entryPoint)
    {
        EntryPoint = entryPoint;
    }
}
