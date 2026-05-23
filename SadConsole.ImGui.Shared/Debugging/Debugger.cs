using System;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem.Rendering;
using System.Collections.Generic;

namespace SadConsole.Debug;

/// <summary>
/// The SadConsole debugger class.
/// </summary>
public partial class ImGuiDebugger: ImGuiObjectBase
{
    private bool _firstStart = true;

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
    public event Action<bool, ImGuiRenderer> Opened;

    /// <summary>
    /// An event that's raised when the debugger is closed.
    /// </summary>
    public event Action Closed;

    public GuiTopBar VisualTopBar = new GuiTopBar();
    public GuiDockspace VisualDockspace = new GuiDockspace();
    public GuiScreenObjects VisualScreenObjects = new GuiScreenObjects();
    public GuiPreviews VisualPreviews = new GuiPreviews();

    /// <summary>
    /// The list of UI elements that make up the debugger. This is used for iterating through the debugger's UI when building it.
    /// </summary>
    public List<ImGuiObjectBase> DebuggerUI;

    private ImGuiDebugger()
    {
        ScreenObjectDetailsPanel.RegisteredPanels.Add(typeof(UI.Window), new Editors.ScreenObjectEditorWindowConsole());
        ComponentsPanel.RegisteredPanels.Add(typeof(Components.Cursor), new Editors.ComponentEditorCursor());
        ComponentsPanel.RegisteredPanels.Add(typeof(Components.LayeredSurface), new Editors.ComponentEditorLayeredSurface());
        ComponentsPanel.RegisteredPanels.Add(typeof(UI.ControlHost), new Editors.ComponentEditorControlHost());

        IsVisible = false;

        VisualTopBar.Closed += VisualTopBar_Closed;

        DebuggerUI = [VisualTopBar, VisualDockspace, VisualScreenObjects, VisualPreviews];
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
        SadConsole.Settings.DoUpdate = false;

        Instance.IsVisible = true;

        GuiState.RefreshScreenObject();

        Instance.Opened?.Invoke(Instance._firstStart, renderer);
        Instance._firstStart = false;
    }

    /// <summary>
    /// Stops the debugger.
    /// </summary>
    public static void Stop()
    {
        SadConsole.Settings.DoFinalDraw = true;
        SadConsole.Settings.DoUpdate = true;
        Host.Global.BlockSadConsoleInput = false;

        Instance.IsVisible = false;

        Instance.Closed?.Invoke();
    }

    private static void ImGuiCore_ImGuiComponent_HostClosed(object sender, EventArgs e) =>
        Stop();

    /// <summary>
    /// Draws the debugging interface if <see cref="ImGuiObjectBase.IsVisible"/> is true.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsVisible)
        {
            foreach (ImGuiObjectBase element in DebuggerUI)
                element.BuildUI(renderer);
        }
    }
}
