using System;
using System.Collections.Generic;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;
using SadConsole.Debug.ScreenObjectEditors;

namespace SadConsole.Debug;

/// <summary>
/// The SadConsole debugger class.
/// </summary>
public static partial class Debugger
{
    private static ImGuiMonoGameComponent _imGui;

    /// <summary>
    /// An event that's raised when the debugger is opened. True is passed if it's the first time it's opened.
    /// </summary>
    public static event Action<bool> Opened;
    /// <summary>
    /// An event that's raised when the debugger is closed.
    /// </summary>
    public static event Action Closed;

    /// <summary>
    /// The ImGui objects to draw each game frame.
    /// </summary>
    public static List<ImGuiObjectBase> GuiComponents => _imGui.UIComponents;

    /// <summary>
    /// The ImGui renderer.
    /// </summary>
    public static ImGuiRenderer Renderer => _imGui.ImGuiRenderer;

    /// <summary>
    /// True when the debugger is currently opened.
    /// </summary>
    public static bool IsOpened
    {
        get
        {
            if (_imGui == null) return false;
            return _imGui.Visible;
        }
    }

    /// <summary>
    /// Starts the debugger.
    /// </summary>
    public static void Start()
    {
        if (_imGui != null)
        {
            _imGui.Visible = true;
            _imGui.Enabled = true;

            Host.Global.SadConsoleComponent.Enabled = false;
            GuiState.RefreshScreenObject();
            Opened?.Invoke(false);
            return;
        }

        Host.Global.SadConsoleComponent.Enabled = false;
        GuiState.RefreshScreenObject();

        _imGui = new ImGuiMonoGameComponent(Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
        var ptr = ImGui.GetIO().Fonts.AddFontFromFileTTF(@"Roboto-Regular.ttf", 14);
        _imGui.ImGuiRenderer.SetDefaultFont(ptr);
        _imGui.HostClosed += _imGui_HostClosed;

        Themes.SetModernColors();

        _imGui.UIComponents.Add(new GuiTopBar());
        _imGui.UIComponents.Add(new GuiDockspace());
        _imGui.UIComponents.Add(new ScreenObjectsPanel());
        _imGui.UIComponents.Add(new GuiPreviews());

        ScreenObjectDetailsPanel.RegisteredPanels.Add(typeof(UI.Window), new WindowConsolePanel());
        ComponentsPanel.RegisteredPanels.Add(typeof(Components.Cursor), new SadComponentEditors.ComponentEditorCursor());

        _imGui.Update(new GameTime());
        Game.Instance.MonoGameInstance.Components.Add(_imGui);
        Opened?.Invoke(true);
    }

    /// <summary>
    /// Stops the debugger.
    /// </summary>
    public static void Stop()
    {
        Host.Global.SadConsoleComponent.Enabled = true;
        Host.Global.BlockSadConsoleInput = false;

        _imGui.Visible = false;
        _imGui.Enabled = false;

        SadConsole.Settings.Input.DoKeyboard = true;
        SadConsole.Settings.Input.DoMouse = true;

        Closed?.Invoke();
    }

    private static void _imGui_HostClosed(object sender, EventArgs e) =>
        Stop();
}
