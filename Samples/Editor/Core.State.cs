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

        }

        public static GuiTopBar GuiTopBar = new();
        public static GuiDockSpace GuiDockSpace = new();

        public static ImGuiList<Documents.Document> Documents = new();

        public static ImGuiList<Documents.IBuilder> DocumentBuilders = new(new Documents.DocumentSurface.Builder());

        public static ImGuiList<Tools.ITool> Tools = new();

        public static EditorPalette Palette = new();

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
