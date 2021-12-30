using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole;
using SadConsole.Editor.GuiParts;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor
{
    public static class ImGuiCore
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

        public static GuiParts.GuiTopBar GuiTopBar;
        public static GuiParts.GuiDockspace GuiDockspace;

        /// <summary>
        /// Initializes the debugger.
        /// </summary>
        public static void BasicInit()
        {
            _imGui = new ImGuiMonoGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
            _imGui.Font = "Roboto-Regular.ttf";
            _imGui.fontSize = 14f;

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
                SadConsole.Settings.DoFinalDraw = false;

                return;
            }

            SadConsole.Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
            SadConsole.Settings.DoFinalDraw = false;

            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = false;
            //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = false;

            _imGui = new ImGuiMonoGameComponent(SadConsole.Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
            _imGui.Font = "Roboto-Regular.ttf";
            _imGui.fontSize = 14f;
            _imGui.HostClosed += _imGui_HostClosed;

            GuiTopBar = new();
            GuiDockspace = new();

            ResetUIList();

            //GuiState.GuiFinalOutputWindow = new FinalOutputWindow("Output preview", true);
            //_imGui.UIComponents.Add(GuiState.GuiFinalOutputWindow);

            //_debuggerComponent = new DebuggerComponent(Game.Instance.MonoGameInstance, imGui);
            Game.Instance.MonoGameInstance.Components.Add(_imGui);

            //ImGuiNET.ImGui.
        }

        public static void ResetUIList()
        {
            _imGui.UIComponents.Clear();
            _imGui.UIComponents.Add(GuiTopBar);
            _imGui.UIComponents.Add(GuiDockspace);
        }

        public static void ShowCreateDocument()
        {
            GuiNewFileWindow window = new GuiNewFileWindow();
            window.IsOpen = true;
            ImGuiCore.GuiComponents.Add(window);
            window.Closed += (s, e) =>
            {

            };
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
