// Main entry point for the CFG Visualizer application

using System.Drawing;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using VibeDisasm.CfgVisualizer;

var window = Window.Create(WindowOptions.Default with
{
    Title = "VibeDisasm CFG Visualizer",
    Size = new Vector2D<int>(1280, 720)
});

// Declare variables
ImGuiController controller = null!;
GL gl = null!;
IInputContext inputContext = null!;

window.IsEventDriven = true;

var app = new App();

// Loading function
window.Load += () =>
{
    var openGl = window.CreateOpenGL();

    ImFontPtr mainFont = null;

    controller = new ImGuiController(
        gl = openGl, // load OpenGL
        window, // pass in our window
        inputContext = window.CreateInput(), // create an input context
        onConfigureIO: () =>
        {
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            // Load font if available, otherwise use default
            try
            {
                mainFont = io.Fonts.AddFontFromFileTTF(
                    filename: "assets/Font/OpenSans-Regular.ttf",
                    size_pixels: 18,
                    font_cfg: null,
                    glyph_ranges: io.Fonts.GetGlyphRangesCyrillic()
                );
            }
            catch
            {
                Console.WriteLine("Warning: Could not load font. Using default font.");
            }
        }
    );

    app.Init(window, openGl, mainFont);

    // Setup keyboard event handlers
    inputContext.Keyboards[0]
        .KeyDown += (keyboard, key, scancode) => { app.OnKeyDown(key); };
    inputContext.Keyboards[0]
        .KeyUp += (keyboard, key, scancode) => { app.OnKeyPressed(key); };
    inputContext.Keyboards[0]
        .KeyUp += (keyboard, key, scancode) => { app.OnKeyReleased(key); };
};

// Handle resizes
window.FramebufferResize += s =>
{
    // Adjust the viewport to the new window size
    gl.Viewport(s);
};

// Handle file drops
window.FileDrop += paths =>
{
    app.OnFileDrop(paths);
};

window.Update += delta =>
{
    // Make sure ImGui is up-to-date
    controller.Update((float)delta);

    app.Update(delta);
};

// The render function
window.Render += delta =>
{
    // Background color
    gl.ClearColor(
        Color.FromArgb(
            255,
            (int)(.45f * 255),
            (int)(.55f * 255),
            (int)(.60f * 255)
        )
    );
    gl.Clear((uint)ClearBufferMask.ColorBufferBit);

    app.OnImGuiRender();

    // Make sure ImGui renders too!
    controller.Render();
};

// The closing function
window.Closing += () =>
{
    app.Exit();

    ImGui.SaveIniSettingsToDisk("imgui.ini");

    // Dispose our controller first
    controller?.Dispose();

    // Dispose the input context
    inputContext?.Dispose();

    // Unload OpenGL
    gl?.Dispose();
};

// Run the application
window.Run();
