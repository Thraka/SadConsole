using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class DocumentLayeredSurface
{
    public class Builder : IBuilder
    {
        public string Name;
        public int Width;
        public int Height;
        public int LayerCount;
        public Vector4 DefaultForeground;
        public Vector4 DefaultBackground;

        public string Title => "Layered Surface";

        public Builder() =>
            ResetBuilder();

        public void ImGuiNewDocument(ImGuiRenderer renderer)
        {
            float paddingX = ImGui.GetStyle().WindowPadding.X;
            float windowWidth = ImGui.GetWindowWidth();

            ImGui.Text("Name");
            ImGui.InputText("##name", ref Name, 50);

            ImGui.Separator();

            if (SettingsTable.BeginTable("new_doc_table"))
            {
                SettingsTable.DrawInt("Layers", "##doclayercount", ref LayerCount, 1, 50);
                SettingsTable.DrawInt("Width", "##docwidth", ref Width, 1, 2000);
                SettingsTable.DrawInt("Height", "##docheight", ref Height, 1, 2000);
                SettingsTable.DrawColor("Def. Foreground", "##fore", ref DefaultForeground, Color.White.ToVector4(), true, out _);
                SettingsTable.DrawColor("Def. Background", "##back", ref DefaultBackground, Color.Black.ToVector4(), true, out _);

                SettingsTable.EndTable();
            }
        }

        public bool IsDocumentValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && Width > 0 && Height > 0 && LayerCount > 0;
        }

        public Document CreateDocument()
        {
            LayeredScreenSurface layeredSurface = new(Width, Height);
            layeredSurface.Surface.DefaultForeground = DefaultForeground.ToColor();
            layeredSurface.Surface.DefaultBackground = DefaultBackground.ToColor();
            layeredSurface.Surface.Clear();

            for (int i = 1; i < LayerCount; i++)
            {
                var layer = layeredSurface.Layers.Create();
                layer.DefaultForeground = DefaultForeground.ToColor();
                layer.DefaultBackground = Color.Transparent;
                layer.Clear();
            }

            return new DocumentLayeredSurface(layeredSurface) { Title = Name };
        }

        public void ResetBuilder()
        {
            Name = Document.GenerateName("LayeredSurface");
            Width = 80;
            Height = 25;
            LayerCount = 1;
            DefaultForeground = Color.White.ToVector4();
            DefaultBackground = Color.Black.ToVector4();
        }

        public IEnumerable<IFileHandler> GetLoadHandlers() =>
            [new LayeredSurfaceDocument(), new LayeredSurfaceFile()];

        public override string ToString() =>
            Title;
    }
}
