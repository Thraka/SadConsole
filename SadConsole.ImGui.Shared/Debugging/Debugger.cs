using System;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Debug;

/// <summary>
/// The SadConsole debugger class.
/// </summary>
public partial class ImGuiDebugger: ImGuiObjectBase
{
    /// <summary>
    /// Gets the singleton instance of the ImGuiDebugger class, providing access to its debugging functionalities.
    /// </summary>
    public static ImGuiDebugger Instance { get; private set; } = new ImGuiDebugger();

    /// <summary>
    /// Gets a value indicating whether the associated element is currently opened.
    /// </summary>
    public bool IsOpened => IsVisible;

    /// <summary>
    /// An event that's raised when the debugger is opened. True is passed if it's the first time it's opened.
    /// </summary>
    public event Action<bool> Opened;

    /// <summary>
    /// An event that's raised when the debugger is closed.
    /// </summary>
    public event Action Closed;

    public GuiTopBar VisualTopBar = new GuiTopBar();
    public GuiDockspace VisualDockspace = new GuiDockspace();
    public GuiScreenObjects VisualScreenObjects = new GuiScreenObjects();
    public GuiPreviews VisualPreviews = new GuiPreviews();

    private ImGuiDebugger()
    {
        ScreenObjectDetailsPanel.RegisteredPanels.Add(typeof(UI.Window), new Editors.ScreenObjectEditorWindowConsole());
        ComponentsPanel.RegisteredPanels.Add(typeof(Components.Cursor), new Editors.ComponentEditorCursor());
        ComponentsPanel.RegisteredPanels.Add(typeof(Components.LayeredSurface), new Editors.ComponentEditorLayeredSurface());
        ComponentsPanel.RegisteredPanels.Add(typeof(UI.ControlHost), new Editors.ComponentEditorControlHost());

        IsVisible = false;

        VisualTopBar.Closed += VisualTopBar_Closed;
    }

    private void VisualTopBar_Closed()
    {
        Stop();
    }

    /// <summary>
    /// Starts the debugger.
    /// </summary>
    public static void Start(ImGuiRenderer renderer)
    {
        SadConsole.Settings.DoFinalDraw = false;

        Instance.IsVisible = true;

        GuiState.RefreshScreenObject();

        Instance.Opened?.Invoke(false);
    }

    /// <summary>
    /// Stops the debugger.
    /// </summary>
    public static void Stop()
    {
        SadConsole.Settings.DoFinalDraw = true;
        Host.Global.BlockSadConsoleInput = false;

        Instance.IsVisible = false;

        Instance.Closed?.Invoke();
    }

    private static void ImGuiCore_ImGuiComponent_HostClosed(object sender, EventArgs e) =>
        Stop();

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsVisible)
        {
            VisualTopBar.BuildUI(renderer);
            VisualDockspace.BuildUI(renderer);
            VisualScreenObjects.BuildUI(renderer);
            VisualPreviews.BuildUI(renderer);
        }
    }
}
