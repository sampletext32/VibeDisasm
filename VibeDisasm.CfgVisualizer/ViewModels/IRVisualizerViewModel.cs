using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.Services;
using VibeDisasm.DecompilerEngine.IREverything.Model;

namespace VibeDisasm.CfgVisualizer.ViewModels;

/// <summary>
/// View model for the IR visualizer panel
/// </summary>
public class IRVisualizerViewModel : IViewModel
{
    private readonly IRVisualizationService _irVisualizationService;
    private IRFunction? _currentFunction;
    private bool _showBasicBlocks = true;
    private bool _showIfThenNodes = true;
    private bool _showIfThenElseNodes = true;
    private bool _colorCodeNodes = true;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="irVisualizationService">Service for managing IR functions</param>
    public IRVisualizerViewModel(IRVisualizationService irVisualizationService)
    {
        _irVisualizationService = irVisualizationService;
        _irVisualizationService.FunctionsChanged += OnFunctionsChanged;
    }

    /// <summary>
    /// The currently loaded IR function
    /// </summary>
    public IRFunction? CurrentFunction
    {
        get => _currentFunction;
        set
        {
            if (_currentFunction != value)
            {
                _currentFunction = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Whether to show basic blocks in the visualization
    /// </summary>
    public bool ShowBasicBlocks
    {
        get => _showBasicBlocks;
        set
        {
            if (_showBasicBlocks != value)
            {
                _showBasicBlocks = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Whether to show if-then nodes in the visualization
    /// </summary>
    public bool ShowIfThenNodes
    {
        get => _showIfThenNodes;
        set
        {
            if (_showIfThenNodes != value)
            {
                _showIfThenNodes = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Whether to show if-then-else nodes in the visualization
    /// </summary>
    public bool ShowIfThenElseNodes
    {
        get => _showIfThenElseNodes;
        set
        {
            if (_showIfThenElseNodes != value)
            {
                _showIfThenElseNodes = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Whether to color-code nodes by type
    /// </summary>
    public bool ColorCodeNodes
    {
        get => _colorCodeNodes;
        set
        {
            if (_colorCodeNodes != value)
            {
                _colorCodeNodes = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets all available IR functions
    /// </summary>
    public IReadOnlyList<IRFunction> AvailableFunctions => _irVisualizationService.Functions;

    /// <summary>
    /// Sets the current function to visualize
    /// </summary>
    /// <param name="function">The IR function to visualize</param>
    [Pure]
    public void SetFunction(IRFunction function)
    {
        CurrentFunction = function;
    }

    private void OnFunctionsChanged(object? sender, EventArgs e)
    {
        // If we have functions but no current function, select the first one
        if (CurrentFunction is null && _irVisualizationService.Functions.Count > 0)
        {
            CurrentFunction = _irVisualizationService.Functions[0];
        }
        // If our current function is no longer in the list, clear it
        else if (CurrentFunction is not null && !_irVisualizationService.Functions.Contains(CurrentFunction))
        {
            CurrentFunction = _irVisualizationService.Functions.Count > 0 ? 
                _irVisualizationService.Functions[0] : null;
        }

        OnPropertyChanged(nameof(AvailableFunctions));
    }

    /// <summary>
    /// Property changed event
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    /// <param name="propertyName">Name of the property that changed</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
