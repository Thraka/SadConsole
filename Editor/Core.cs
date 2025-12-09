using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Utilities;
using Newtonsoft.Json.Linq;
using SadConsole.Editor.GuiObjects;
using SadConsole.Editor.Tools;
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

        public Color EmptyCellColor = Color.NavajoWhite;
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
                .AddFontFromFileTTF(Settings.Font, Settings.FontSize, [0x01, 0xFFFFF])
                .Build()
        );

        Core.State.LoadEditorPalette();

        ImGui.GetStyle().ScaleAllSizes(Settings.UIScale);

        ResetUIList();

        Game.Instance.MonoGameInstance.Components.Add(ImGuiCore.ImGuiComponent);

        // Test code
        // ===============
        //Documents.DocumentAnimated.Builder builder = new();
        //builder.ResetBuilder();
        //builder.FrameCount = 5;

        /*
        Documents.DocumentSurface.Builder builder = new();
        builder.ResetBuilder();

        State.Documents.Add(builder.CreateDocument());
        State.Documents.SelectedItemIndex = 0;
        State.Documents.SelectedItem!.OnSelected();
        State.Tools.SelectedItemIndex = 0;

        ((Documents.DocumentSurface)State.Documents.SelectedItem).Zones.Objects.Add(new ZoneSimplified()
        {
            Name = "Default Zone",
            ZoneArea = new(new SadRogue.Primitives.Rectangle(1, 1, 10, 10).Positions()),
            Appearance = new ColoredGlyph()
            {
                Foreground = SadRogue.Primitives.Color.Yellow,
                Background = SadRogue.Primitives.Color.DarkGray,
                Glyph = '@',
                Mirror = SadConsole.Mirror.None
            },
            Settings = new Dictionary<string, string>()
            {
                { "IsBlocking", "true" },
                { "TriggerOnEnter", "false" },
                { "ZoneType", "SafeArea" },
                { "Description", "A default zone for testing" },
                { "Priority", "1" }
            }
        });

        ((Documents.DocumentSurface)State.Documents.SelectedItem).SimpleObjects.Objects.Add(new SimpleObjectDefinition()
        {
            Name = "Tree",
            Visual = new ColoredGlyph()
            {
                Foreground = SadRogue.Primitives.Color.DarkGreen.GetDarker(),
                Background = SadRogue.Primitives.Color.AnsiGreenBright,
                Glyph = (char)6,
                Mirror = SadConsole.Mirror.None
            }
        });

        State.Documents.SelectedItem.Options.UseSimpleObjects = true;
        State.Documents.SelectedItem.Options.UseZones = true;
        State.Documents.SelectedItem.SyncToolModes();
        */
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
