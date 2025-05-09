using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Microsoft.Extensions.DependencyInjection;
using VibeDisasm.CfgVisualizer.Abstractions;
using VibeDisasm.CfgVisualizer.ImGuiUI;
using VibeDisasm.CfgVisualizer.Models;
using VibeDisasm.CfgVisualizer.Services;

namespace VibeDisasm.CfgVisualizer;

/// <summary>
/// Main application class that manages the ImGui panels and application lifecycle
/// </summary>
public class App : IUpdateReceiver, IKeyPressReceiver, IExitReceiver
{
    public GL GL { get; set; } = null!;
    public IWindow Window { get; set; } = null!;
    public ImFontPtr MainFont { get; set; }

    public static App Instance { get; private set; } = null!;

    // Dockspace configuration
    private static bool _dockspaceOpen = true;
    private static bool _optFullscreenPersistant = true;
    private static bool _optFullscreen = _optFullscreenPersistant;
    private static ImGuiDockNodeFlags _dockspaceFlags = ImGuiDockNodeFlags.None;

    private List<IImGuiPanel> _imGuiPanels = [];
    private ActionsService _actionsService;

    /// <summary>
    /// Constructor
    /// </summary>
    public App()
    {
        Instance = this;
    }

    /// <summary>
    /// Initialize the application
    /// </summary>
    /// <param name="window">Window instance</param>
    /// <param name="gl">OpenGL instance</param>
    /// <param name="mainFont">ImGui font</param>
    public void Init(IWindow window, GL gl, ImFontPtr mainFont)
    {
        // Store references
        Window = window;
        GL = gl;
        MainFont = mainFont;

        // Setup ImGui style
        ImGui.StyleColorsLight();

        IServiceCollection serviceCollection = new ServiceCollection();

        foreach (var type in Utils.GetAssignableTypes<IImGuiPanel>())
        {
            serviceCollection.AddSingleton(type);
        }

        serviceCollection.AddSingleton(gl);
        serviceCollection.AddSingleton(window);
        
        foreach (var service in Utils.GetAssignableTypes<IService>())
        {
            serviceCollection.AddSingleton(service);
        }

        foreach (var service in Utils.GetAssignableTypes<IViewModel>())
        {
            serviceCollection.AddSingleton(service);
        }

        var serviceProvider = serviceCollection.BuildServiceProvider();

        _imGuiPanels = Utils.GetAssignableTypes<IImGuiPanel>()
            .Select(t => (serviceProvider.GetService(t) as IImGuiPanel)!)
            .ToList();

        _actionsService = serviceProvider.GetRequiredService<ActionsService>();
    }

    /// <summary>
    /// Handle file drop events
    /// </summary>
    /// <param name="paths">Dropped file paths</param>
    public void OnFileDrop(string[] paths)
    {
        if (paths.Length > 0)
        {
            // Check if the file has a valid extension
            string extension = Path.GetExtension(paths[0]).ToLowerInvariant();
            if (extension is ".exe" or ".dll")
            {
                _actionsService.TryLoadFile(paths[0]);
            }
            else
            {
                Console.WriteLine($"Unsupported file type: {extension}");
            }
        }
    }

    /// <summary>
    /// Render ImGui UI
    /// </summary>
    public void OnImGuiRender()
    {
        // Push font if available
        if (MainFont.IsLoaded())
        {
            ImGui.PushFont(MainFont);
        }

        // Setup dockspace
        var windowFlags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        if (_optFullscreen)
        {
            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize |
                            ImGuiWindowFlags.NoMove;
            windowFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        }

        // When using ImGuiDockNodeFlags_PassthruCentralNode, DockSpace() will render our background and handle the pass-thru hole
        if ((_dockspaceFlags & ImGuiDockNodeFlags.PassthruCentralNode) != 0)
            windowFlags |= ImGuiWindowFlags.NoBackground;

        // Important: note that we proceed even if Begin() returns false (aka window is collapsed).
        // This is because we want to keep our DockSpace() active. If a DockSpace() is inactive, 
        // all active windows docked into it will lose their parent and become undocked.
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
        ImGui.Begin("DockSpace", ref _dockspaceOpen, windowFlags);
        ImGui.PopStyleVar();

        if (_optFullscreen)
            ImGui.PopStyleVar(2);

        // DockSpace
        var io = ImGui.GetIO();
        var style = ImGui.GetStyle();
        var minWinSizeX = style.WindowMinSize.X;
        style.WindowMinSize.X = 370.0f;
        if ((io.ConfigFlags & ImGuiConfigFlags.DockingEnable) != 0)
        {
            var dockspaceId = ImGui.GetID("MainDockSpace");
            ImGui.DockSpace(dockspaceId, new Vector2(0.0f, 0.0f), _dockspaceFlags);
        }

        style.WindowMinSize.X = minWinSizeX;

        // Render all panels
        foreach (var imGuiPanel in _imGuiPanels)
        {
            imGuiPanel.OnImGuiRender();
        }

        // Pop font if pushed
        if (MainFont.IsLoaded())
        {
            ImGui.PopFont();
        }

        ImGui.End();
    }

    /// <summary>
    /// Update the application
    /// </summary>
    /// <param name="deltaTime">Time since last frame</param>
    public void Update(double deltaTime)
    {
        // Update logic here if needed
    }

    /// <summary>
    /// Handle key press events
    /// </summary>
    /// <param name="key">Pressed key</param>
    public void OnKeyPressed(Key key)
    {
        // Handle key press events
    }

    /// <summary>
    /// Handle key down events
    /// </summary>
    /// <param name="key">Key being held down</param>
    public void OnKeyDown(Key key)
    {
        // Handle key down events
    }

    /// <summary>
    /// Handle key release events
    /// </summary>
    /// <param name="key">Released key</param>
    public void OnKeyReleased(Key key)
    {
        // Handle key release events
    }

    /// <summary>
    /// Clean up resources when the application exits
    /// </summary>
    public void Exit()
    {
        // Cleanup resources
    }
}
