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

        public static ImGuiList<Documents.Document> Documents = new();

        public static ImGuiList<Documents.IBuilder> DocumentBuilders = new(new Documents.DocumentSurface.Builder(), new Documents.DocumentAnimated.Builder());

        public static ImGuiList<Tools.ITool> Tools = new();

        public static EditorPalette Palette = new();

        public static ImGuiList<Blueprint> Blueprints = new();
        public static ImGuiList<IFont> SadConsoleFonts = new();

        public static void LoadBlueprints()
        {
            List<Blueprint> blueprints = new();

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
            SadConsoleFonts = new(SadConsole.Game.Instance.Fonts.Values)
            {
                SelectedItemIndex = 0
            };
        }

        public static void SyncEditorPalette()
        {
            Hexa.NET.ImGui.SC.Windows.PalettePopup.ExtraPalettes.Clear();
            Hexa.NET.ImGui.SC.Windows.PalettePopup.ExtraPalettes.Add(new Tuple<string, NamedColor[]>("Editor", Palette.Colors));

            if (Documents.IsItemSelected() && Documents.SelectedItem.HasPalette)
                Hexa.NET.ImGui.SC.Windows.PalettePopup.ExtraPalettes.Add(new Tuple<string, NamedColor[]>("Document",
                    Documents.SelectedItem.Palette.Colors));
        }
    }
}
