using SadConsole.Editor.GuiParts;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor;

public static partial class ImGuiCore
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

    public static GuiStartup GuiStartup;
    public static GuiTopBar GuiTopBar;
    public static GuiDockspace GuiDockspace;
    public static WindowActiveDocuments GuiSidePane;
    public static WindowDocumentsHost GuiDocumentsHost;
    public static WindowTools GuiToolsWindow;

    private static DebuggingTools _debuggingTools;

    /// <summary>
    /// Initializes the debugger.
    /// </summary>
    public static void BasicInit()
    {
        _imGui = new ImGuiMonoGameComponent(Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);
        //_imGui.Font = "Roboto-Regular.ttf";
        //_imGui.fontSize = 14f;

        Game.Instance.MonoGameInstance.Components.Add(_imGui);
        Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
    }

    public static void Start()
    {
        if (_imGui != null)
        {
            _imGui.Visible = true;
            _imGui.Enabled = true;

            Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
            Settings.DoFinalDraw = false;

            return;
        }

        Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = false;
        Settings.DoFinalDraw = false;

        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = false;
        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = false;

        _imGui = new ImGuiMonoGameComponent(Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);

        var value = _imGui.ImGuiRenderer.AddFontTTF("JetBrains Mono SemiBold Nerd Font Complete.ttf", 16f);
        _imGui.ImGuiRenderer.SetDefaultFont(value);

        _imGui.HostClosed += _imGui_HostClosed;

        //var ptr = ImGuiNET.ImGui.GetIO().Fonts.AddFontFromFileTTF(@"C:\Windows\Fonts\ARIAL.TTF", 13);
        //_imGui.ImGuiRenderer.RebuildFontAtlas();
        //ImGuiNET.ImGui.PushFont(ptr);

        //ImGuiNET.ImGui.GetStyle().ScaleAllSizes(2f);

        GuiStartup = new();
        GuiTopBar = new();
        GuiDockspace = new();
        GuiDocumentsHost = new();
        GuiSidePane = new();
        GuiToolsWindow = new();
        _debuggingTools = new();

        ResetUIList();

        //GuiState.GuiFinalOutputWindow = new FinalOutputWindow("Output preview", true);
        //_imGui.UIComponents.Add(GuiState.GuiFinalOutputWindow);

        //_debuggerComponent = new DebuggerComponent(Game.Instance.MonoGameInstance, imGui);
        Game.Instance.MonoGameInstance.Components.Add(_imGui);

        //ImGuiNET.ImGui.

        // Test code
        var doc = Model.SurfaceDocument.FromSettings(280, 225, Color.White, Color.Black);
        State.OpenDocuments = [.. State.OpenDocuments, doc];
        ((Model.SurfaceDocument)State.OpenDocuments[0]).Surface.Surface.View = new Rectangle(0, 0, 10, 10);
    }

    public static void ResetUIList()
    {
        _imGui.UIComponents.Clear();
        _imGui.UIComponents.Add(GuiStartup);
        _imGui.UIComponents.Add(GuiTopBar);
        _imGui.UIComponents.Add(GuiDockspace);
        _imGui.UIComponents.Add(GuiSidePane);
        _imGui.UIComponents.Add(GuiToolsWindow);
        _imGui.UIComponents.Add(GuiDocumentsHost);
        _imGui.UIComponents.Add(_debuggingTools);
    }

    public static void ShowCreateDocument()
    {
        NewFile window = new();
        window.Closed += (s, e) =>
        {
            if (window.DialogResult)
                State.OpenDocuments = [.. State.OpenDocuments, window.Document];
        };
        window.Show();
    }

    public static void Alert(string message)
    {
        SaveFileError window = new(message);
        window.Show();
    }

    private static void _imGui_HostClosed(object? sender, EventArgs e) =>
        Stop();

    public static void Stop()
    {
        Game.Instance.MonoGameInstance.SadConsoleComponent.Visible = true;
        Game.Instance.MonoGameInstance.SadConsoleComponent.Enabled = true;
        Settings.DoFinalDraw = true;

        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = true;
        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = true;

        _imGui.Visible = false;
        _imGui.Enabled = false;

        Settings.Input.DoKeyboard = true;
        Settings.Input.DoMouse = true;
    }
}
