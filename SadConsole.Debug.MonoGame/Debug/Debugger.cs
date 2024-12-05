using System;
using System.Collections.Generic;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;
using Microsoft.Xna.Framework;

namespace SadConsole.Debug;

public static partial class Debugger
{
    private static ImGuiMonoGameComponent _imGui;

    public static List<ImGuiObjectBase> GuiComponents => _imGui.UIComponents;

    public static ImGuiRenderer Renderer => _imGui.ImGuiRenderer;

    public static bool IsOpened
    {
        get
        {
            if (_imGui == null) return false;
            return _imGui.Visible;
        }
    }

    /// <summary>
    /// Initializes the debugger.
    /// </summary>
    public static void BasicInit()
    {
        _imGui = new ImGuiMonoGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, (Microsoft.Xna.Framework.Game)Game.Instance.MonoGameInstance, true);
        //_imGui.Font = "Roboto-Regular.ttf";
        //_imGui.fontSize = 14f;

        Game.Instance.MonoGameInstance.Components.Add(_imGui);
        Host.Global.SadConsoleComponent.Enabled = false;
    }

    public static void Start()
    {
        if (_imGui != null)
        {
            _imGui.Visible = true;
            _imGui.Enabled = true;

            Host.Global.SadConsoleComponent.Enabled = false;
            return;
        }

        Host.Global.SadConsoleComponent.Enabled = false;

        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = false;
        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = false;

        _imGui = new ImGuiMonoGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, (Microsoft.Xna.Framework.Game)Game.Instance.MonoGameInstance, true);
        var ptr = ImGui.GetIO().Fonts.AddFontFromFileTTF(@"Roboto-Regular.ttf", 14);
        _imGui.ImGuiRenderer.SetDefaultFont(ptr);
        _imGui.HostClosed += _imGui_HostClosed;

        ImGuiSystem.Themes.SetModernColors();

        _imGui.UIComponents.Add(new GuiTopBar());
        _imGui.UIComponents.Add(new GuiDockspace());
        _imGui.UIComponents.Add(new ScreenObjectsPanel());
        //_imGui.UIComponents.Add(new GuiSurfacePreview());
        _imGui.UIComponents.Add(new GuiPreviews());

        ScreenObjectDetailsPanel.RegisteredPanels.Add(typeof(SadConsole.UI.Window), new SadComponentEditors.WindowConsolePanel());
        ComponentsPanel.RegisteredPanels.Add(typeof(SadConsole.Components.Cursor), new SadComponentEditors.ComponentEditorCursor());

        _imGui.Update(new GameTime());
        Game.Instance.MonoGameInstance.Components.Add(_imGui);
    }

    private static void _imGui_HostClosed(object sender, EventArgs e) =>
        Stop();

    public static void Stop()
    {
        Host.Global.SadConsoleComponent.Visible = true;
        Host.Global.SadConsoleComponent.Enabled = true;
        Host.Global.BlockSadConsoleInput = false;
        SadConsole.Settings.DoFinalDraw = true;

        _imGui.Visible = false;
        _imGui.Enabled = false;

        SadConsole.Settings.Input.DoKeyboard = true;
        SadConsole.Settings.Input.DoMouse = true;
    }
}
