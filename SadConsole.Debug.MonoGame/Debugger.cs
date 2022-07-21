using System;
using System.Collections.Generic;
using System.Text;

namespace SadConsole.Debug.MonoGame
{
    public static class Debugger
    {

/* Unmerged change from project 'SadConsole.Debug.MonoGame (net6.0)'
Before:
        private static MyraUI.MyraGameComponent _myraGameComponent;
After:
        private static MyraGameComponent _myraGameComponent;
*/
        private static MyraUI.MyraGameComponent _myraGameComponent;

        public static bool IsOpened
        {
            get
            {
                if (_myraGameComponent == null) return false;
                return _myraGameComponent.Visible;
            }
        }

        //private static CoolTheme coolTheme = new CoolTheme();

        /// <summary>
        /// Initializes the debugger.
        /// </summary>
        public static void BasicInit()
        {
            _myraGameComponent = new MyraUI.MyraGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance);

            Game.Instance.MonoGameInstance.Components.Add(_myraGameComponent);
            SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
        }

        public static void Start()
        {
            if (_myraGameComponent != null)
            {
                _myraGameComponent.Visible = true;
                _myraGameComponent.Enabled = true;

                SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
                SadConsole.Settings.DoFinalDraw = GuiState.ShowSadConsoleRendering;

                return;
            }

            SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
            SadConsole.Settings.DoFinalDraw = GuiState.ShowSadConsoleRendering;

            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = false;
            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = false;

            _myraGameComponent = new MyraUI.MyraGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance);
            _myraGameComponent.HostClosed += _imGui_HostClosed;
            

            //ImGui.Initialize();
            //ImGui.LoadContent();

            GuiState.ShowSadConsoleRenderingChanged += (s, e) => SadConsole.Settings.DoFinalDraw = GuiState.ShowSadConsoleRendering;

            //_imGui.UIComponents.Add(new GuiTopBar());
            //_imGui.UIComponents.Add(new GuiDockspace());
            //_imGui.UIComponents.Add(new GuiScreenObjects());
            //_imGui.UIComponents.Add(new GuiSurfacePreview());

            //GuiState.GuiFinalOutputWindow = new FinalOutputWindow("Output preview", true);
            //_imGui.UIComponents.Add(GuiState.GuiFinalOutputWindow);

            //_debuggerComponent = new DebuggerComponent(Game.Instance.MonoGameInstance, imGui);
            Game.Instance.MonoGameInstance.Components.Add(_myraGameComponent);

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

            _myraGameComponent.Visible = false;
            _myraGameComponent.Enabled = false;

            Settings.Input.DoKeyboard = true;
            Settings.Input.DoMouse = true;
        }
    }
}
