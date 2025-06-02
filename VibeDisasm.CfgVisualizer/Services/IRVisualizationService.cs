using System.Collections.ObjectModel;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.DecompilerEngine.IREverything.Model;

namespace VibeDisasm.CfgVisualizer.Services;

/// <summary>
/// Service for managing IR functions for visualization
/// </summary>
public class IRVisualizationService : IService
{
    private readonly List<IRFunction> _functions = [];
    
    /// <summary>
    /// Gets the collection of available IR functions
    /// </summary>
    public ReadOnlyCollection<IRFunction> Functions => _functions.AsReadOnly();
    
    /// <summary>
    /// Event that fires when the functions collection changes
    /// </summary>
    public event EventHandler<EventArgs> FunctionsChanged;
    
    /// <summary>
    /// Adds an IR function to the collection
    /// </summary>
    /// <param name="function">The function to add</param>
    public void AddFunction(IRFunction function)
    {
        if (!_functions.Contains(function))
        {
            _functions.Add(function);
            FunctionsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    /// <summary>
    /// Clears all functions from the collection
    /// </summary>
    public void ClearFunctions()
    {
        _functions.Clear();
        FunctionsChanged?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Sets the current collection of functions
    /// </summary>
    /// <param name="functions">The functions to set</param>
    public void SetFunctions(IEnumerable<IRFunction> functions)
    {
        _functions.Clear();
        _functions.AddRange(functions);
        FunctionsChanged?.Invoke(this, EventArgs.Empty);
    }
}
