using System;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;

namespace SadConsole.Debug;

/// <summary>
/// The SadConsole debugger class.
/// </summary>
public static partial class Debugger
{

    /// <summary>
    /// An event that's raised when the debugger is opened. True is passed if it's the first time it's opened.
    /// </summary>
    public static event Action<bool> Opened;

    /// <summary>
    /// An event that's raised when the debugger is closed.
    /// </summary>
    public static event Action Closed;


    /// <summary>
    /// True when the debugger is currently opened.
    /// </summary>
    public static bool IsOpened
    {
        get
        {
            if (ImGuiCore.ImGuiComponent == null) return false;
            
            return ImGuiCore.ImGuiComponent.Visible;
        }
    }

    /// <summary>
    /// Starts the debugger.
    /// </summary>
    public static void Start()
    {
        if (ImGuiCore.ImGuiComponent != null)
        {
            ImGuiCore.ImGuiComponent.Visible = true;
            ImGuiCore.ImGuiComponent.Enabled = true;

            Host.Global.SadConsoleComponent.Enabled = false;
            SadConsole.Settings.DoFinalDraw = false;
            GuiState.RefreshScreenObject();
            Opened?.Invoke(false);
            return;
        }

        Host.Global.SadConsoleComponent.Enabled = false;
        SadConsole.Settings.DoFinalDraw = false;
        GuiState.RefreshScreenObject();

        ImGuiCore.ImGuiComponent = new ImGuiMonoGameComponent(Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
        var ptr = ImGui.GetIO().Fonts.AddFontFromFileTTF(@"Roboto-Regular.ttf", 14);
        ImGuiCore.ImGuiComponent.ImGuiRenderer.SetDefaultFont(ptr);
        ImGuiCore.ImGuiComponent.HostClosed += ImGuiCore_ImGuiComponent_HostClosed;

        Themes.SetModernColors();

        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiTopBar());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiDockspace());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiScreenObjects());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiPreviews());

        ScreenObjectDetailsPanel.RegisteredPanels.Add(typeof(UI.Window), new Editors.WindowConsolePanel());
        ComponentsPanel.RegisteredPanels.Add(typeof(Components.Cursor), new Editors.ComponentEditorCursor());
        ComponentsPanel.RegisteredPanels.Add(typeof(Components.LayeredSurface), new Editors.ComponentEditorLayeredSurface());
        ComponentsPanel.RegisteredPanels.Add(typeof(UI.ControlHost), new Editors.ComponentEditorControlHost());

        ImGuiCore.ImGuiComponent.Update(new GameTime());
        Game.Instance.MonoGameInstance.Components.Add(ImGuiCore.ImGuiComponent);
        Opened?.Invoke(true);
    }

    /// <summary>
    /// Stops the debugger.
    /// </summary>
    public static void Stop()
    {
        Host.Global.SadConsoleComponent.Enabled = true;
        Host.Global.BlockSadConsoleInput = false;

        ImGuiCore.ImGuiComponent.Visible = false;
        ImGuiCore.ImGuiComponent.Enabled = false;

        SadConsole.Settings.DoFinalDraw = true;
        SadConsole.Settings.Input.DoKeyboard = true;
        SadConsole.Settings.Input.DoMouse = true;

        Closed?.Invoke();
    }

    private static void ImGuiCore_ImGuiComponent_HostClosed(object sender, EventArgs e) =>
        Stop();
}
