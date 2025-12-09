using System.Diagnostics.CodeAnalysis;
using SadConsole.Editor.GuiObjects;
using SadConsole.ImGuiSystem;
using SadConsole.UI;

namespace SadConsole.Editor;

public static partial class Core
{
    public static class State
    {
        public static class LayoutInfo
        {
            public static float ColorEditBoxWidth = 20;
            public static int TopBarHeight = 0;
        }

        static State()
        {
            LoadBlueprints();
        }

        public static GuiTopBar GuiTopBar = new();
        public static GuiDockSpace GuiDockSpace = new();

        public static List<Documents.Document> Documents = new();

        /// <summary>
        /// The currently selected document for editing. This can be a root document or a child document.
        /// This property is managed by <see cref="GuiDocumentsList"/> and should be used by all editor components.
        /// </summary>
        public static Documents.Document? SelectedDocument { get; set; }

        /// <summary>
        /// Returns true if a document is currently selected.
        /// </summary>
        [MemberNotNullWhen(true, nameof(SelectedDocument))]
        public static bool HasSelectedDocument => SelectedDocument != null;

        public static ImGuiList<Documents.IBuilder> DocumentBuilders = new(new Documents.DocumentSurface.Builder(), new Documents.DocumentLayeredSurface.Builder(), new Documents.DocumentAnimated.Builder(), new Documents.DocumentScene.Builder());

        public static ImGuiList<Tools.ITool> Tools = new();

        public static EditorPalette Palette = new();

        public static ImGuiList<Blueprint> Blueprints = new();
        public static ImGuiList<IFont> SadConsoleFonts = new();

        public static void LoadBlueprints()
        {
            List<Blueprint> blueprints = new();

            if (!Directory.Exists(Core.Settings.BlueprintFolder))
                Directory.CreateDirectory(Core.Settings.BlueprintFolder);

            foreach (string file in Directory.GetFiles(Core.Settings.BlueprintFolder))
            {
                try
                {
                    using BinaryReader reader = new(File.OpenRead(file));

                    string name = reader.ReadString();

                    blueprints.Add(new Blueprint(name, file));
                }
                catch
                {
                }
            }

            Blueprints = new ImGuiList<Blueprint>(blueprints);
        }

        public static void LoadSadConsoleFonts()
        {
            if (!Directory.Exists(Core.Settings.FontsFolder))
                Directory.CreateDirectory(Core.Settings.FontsFolder);

            // Use this way to load fonts because we want SadConsole to use the normal defaults
            Directory.GetFiles(Core.Settings.FontsFolder, "*.font")
                .ToList()
                .ForEach(file => Game.Instance.LoadFont(file));

            SadConsoleFonts = new(Game.Instance.Fonts.Values)
            {
                SelectedItemIndex = 0
            };
        }

        public static void SyncEditorPalette()
        {
            Hexa.NET.ImGui.SC.Windows.PalettePopup.ExtraPalettes.Clear();
            Hexa.NET.ImGui.SC.Windows.PalettePopup.ExtraPalettes.Add(new Tuple<string, NamedColor[]>("Editor", Palette.Colors));

            if (HasSelectedDocument && SelectedDocument!.HasPalette)
                Hexa.NET.ImGui.SC.Windows.PalettePopup.ExtraPalettes.Add(new Tuple<string, NamedColor[]>("Document",
                    SelectedDocument.Palette.Colors));
        }

        public static void LoadEditorPalette()
        {
            if (File.Exists("editor.pal"))
                Palette = EditorPalette.Load("editor.pal");
        }

        public static void SaveEditorPalette()
        {
            if (File.Exists("editor.pal"))
                File.Delete("editor.pal");

            Palette.Save("editor.pal");
        }
    }
}
