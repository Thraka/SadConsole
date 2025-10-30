using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Utilities;
using Newtonsoft.Json.Linq;
using SadConsole.Editor.GuiObjects;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor;

public static partial class Core
{
    public class AppSettings
    {
        public float FontSize = 18f;
        public string Font = "0xProtoNerdFont-Regular.ttf";
        public float UIScale = 1f;

        public string BlueprintFolder = "blueprints";
        public string FontsFolder = "fonts";

        public int WindowNewDocWidthFactor = 22;
        public int WindowSimpleObjectEditor = 40;
        public int WindowGlyphEditor = 20;
    }


    private static ImFontConfig _fontConfig;

    public static AppSettings Settings = new();

    public static void Start()
    {
        if (System.IO.File.Exists("appconfig.json"))
            Settings = SadConsole.Serializer.Load<AppSettings>( "appconfig.json", false);

        SadConsole.Settings.DoFinalDraw = false;

        ImGuiCore.ImGuiComponent = new ImGuiMonoGameComponent(Host.Global.GraphicsDeviceManager, Game.Instance.MonoGameInstance, true);

        //var value = ImGuiCore.ImGuiComponent.ImGuiRenderer.AddFontTTF("JetBrains Mono SemiBold Nerd Font Complete.ttf", 18f);
        //ImGuiCore.ImGuiComponent.ImGuiRenderer.SetDefaultFont(value);
        Themes.SetModernColors();
        ImGuiCore.ImGuiComponent.ImGuiRenderer.SetDefaultFont(
            new ImGuiFontBuilder(ImGui.GetIO().Fonts)
                //.AddFontFromFileTTF("JetBrains Mono SemiBold Nerd Font Complete.ttf", 18f, [0x1, 0x1FFFF])

                //.AddFontFromFileTTF("JetBrains Mono SemiBold Nerd Font Complete.ttf", 18f, [0x1, 0x1FFFF])
                .SetOption(config =>
                {
                    config.FontBuilderFlags |= (uint)ImGuiFreeTypeBuilderFlags.LoadColor;
                    //config.MergeMode = true;
                })
                .AddFontFromFileTTF(Settings.Font, Settings.FontSize, [0x01, 0x1FFFF])
                .Build()
        );


        // _fontConfig = new();
        // _fontConfig.OversampleH = 1;
        // _fontConfig.OversampleV = 1;
        // _fontConfig.MergeMode = 1;
        // _fontConfig.FontBuilderFlags = _fontConfig.FontBuilderFlags | (uint)ImGuiFreeTypeBuilderFlags.LoadColor;
        // uint[] ranges = [ 0x1, 0x1FFFF, 0 ];
        //
        // ImGui.GetIO().Fonts.AddFontFromFileTTF("JetBrains Mono SemiBold Nerd Font Complete.ttf", 18f, ref ranges);

        ImGui.GetStyle().ScaleAllSizes(Settings.UIScale);

        ResetUIList();

        Game.Instance.MonoGameInstance.Components.Add(ImGuiCore.ImGuiComponent);

        // Test code
        // ===============
        Documents.DocumentAnimated.Builder builder = new();
        builder.ResetBuilder();
        builder.FrameCount = 5;
        State.Documents.Objects.Add(builder.CreateDocument());
        State.Documents.SelectedItemIndex = 0;
        State.Documents.SelectedItem!.OnSelected();
        State.Tools.SelectedItemIndex = 0;
    }

    public static void ResetUIList()
    {
        ImGuiCore.ImGuiComponent.UIComponents.Clear();
        ImGuiCore.ImGuiComponent.UIComponents.Add(State.GuiTopBar);
        ImGuiCore.ImGuiComponent.UIComponents.Add(State.GuiDockSpace);
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiDocumentsList());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiToolsList());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiDocumentsHost());
        ImGuiCore.ImGuiComponent.UIComponents.Add(new GuiFinalDrawDocument());
        //ImGuiCore.ImGuiComponent.UIComponents.Add(_debuggingTools);
    }

    public static void ShowCreateDocument()
    {
        //NewFile window = new();
        //window.Closed += (s, e) =>
        //{
        //    if (window.DialogResult)
        //        State.OpenDocuments = [.. State.OpenDocuments, window.Document];
        //};
        //window.Show();
    }

    public static void Alert(string message)
    {
        //MessageWindow window = new(message);
        //window.Show();
    }

    public static void Stop()
    {
        Host.Global.SadConsoleComponent.Enabled = true;
        SadConsole.Settings.DoFinalDraw = true;

        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Visible = true;
        //SadConsole.Game.Instance.MonoGameInstance.ClearScreenComponent.Enabled = true;

        ImGuiCore.ImGuiComponent.Visible = false;
        ImGuiCore.ImGuiComponent.Enabled = false;

        SadConsole.Settings.Input.DoKeyboard = true;
        SadConsole.Settings.Input.DoMouse = true;
    }
}
