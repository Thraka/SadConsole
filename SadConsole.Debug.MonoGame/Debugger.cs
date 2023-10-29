﻿using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.MonoGame
{
    public static class Debugger
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

        //private static CoolTheme coolTheme = new CoolTheme();

        /// <summary>
        /// Initializes the debugger.
        /// </summary>
        public static void BasicInit()
        {
            _imGui = new ImGuiMonoGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
            //_imGui.Font = "Roboto-Regular.ttf";
            //_imGui.fontSize = 14f;

            Game.Instance.MonoGameInstance.Components.Add(_imGui);
            SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
            
        }

        public static void Start()
        {
            if (_imGui != null)
            {
                _imGui.Visible = true;
                _imGui.Enabled = true;

                SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
                SadConsole.Settings.DoFinalDraw = GuiState.ShowSadConsoleRendering;

                return;
            }

            SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
            SadConsole.Settings.DoFinalDraw = GuiState.ShowSadConsoleRendering;

            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = false;
            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = false;

            _imGui = new ImGuiMonoGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
            //_imGui.Font = "Roboto-Regular.ttf";
            //_imGui.fontSize = 14f;
            //ImGui.Theme = coolTheme;
            _imGui.HostClosed += _imGui_HostClosed;
            

            //ImGui.Initialize();
            //ImGui.LoadContent();

            GuiState.ShowSadConsoleRenderingChanged += (s, e) => SadConsole.Settings.DoFinalDraw = GuiState.ShowSadConsoleRendering;

            _imGui.UIComponents.Add(new GuiTopBar());
            _imGui.UIComponents.Add(new GuiDockspace());
            _imGui.UIComponents.Add(new ScreenObjectsPanel());
            _imGui.UIComponents.Add(new GuiSurfacePreview());

            ScreenObjectDetailsPanel.RegisteredPanels.Add(typeof(SadConsole.UI.Window), new WindowConsolePanel());
            ComponentsPanel.RegisteredPanels.Add(typeof(SadConsole.Components.Cursor), new ComponentEditorCursor());

            GuiState.GuiFinalOutputWindow = new FinalOutputWindow("Output preview", true);
            _imGui.UIComponents.Add(GuiState.GuiFinalOutputWindow);

            //_debuggerComponent = new DebuggerComponent(Game.Instance.MonoGameInstance, imGui);
            Game.Instance.MonoGameInstance.Components.Add(_imGui);

            //ImGuiNET.ImGui.
        }

        private static void _imGui_HostClosed(object sender, EventArgs e) =>
            Stop();

        public static void Stop()
        {
            SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Visible = true;
            SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = true;
            SadConsole.Settings.DoFinalDraw = true;

            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = true;
            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = true;

            _imGui.Visible = false;
            _imGui.Enabled = false;

            Settings.Input.DoKeyboard = true;
            Settings.Input.DoMouse = true;
        }
    }
}
