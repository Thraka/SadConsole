using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Documents;

public partial class DocumentSurface
{
    public class Builder : IBuilder
    {
        public string Name;
        public int Width;
        public int Height;
        public Vector4 DefaultForeground;
        public Vector4 DefaultBackground;

        public string Title => "Surface";

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
                SettingsTable.DrawInt("Width", "##docwidth", ref Width, 1, 2000);
                SettingsTable.DrawInt("Height", "##docheight", ref Height, 1, 2000);
                SettingsTable.DrawColor("Def. Foreground", "##fore", ref DefaultForeground, Color.White.ToVector4(), true, out _);
                SettingsTable.DrawColor("Def. Background", "##back", ref DefaultBackground, Color.Black.ToVector4(), true, out _);

                SettingsTable.EndTable();
            }
        }

        public bool IsDocumentValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && Width > 0 && Height > 0;
        }

        public Document CreateDocument()
        {
            CellSurface surface = new(Width, Height);
            surface.DefaultForeground = DefaultForeground.ToColor();
            surface.DefaultBackground = DefaultBackground.ToColor();
            surface.Clear();

            return new DocumentSurface(surface) { Title = Name };
        }

        public void ResetBuilder()
        {
            Name = Document.GenerateName("Surface");
            Width = 80;
            Height = 25;
            DefaultForeground = Color.White.ToVector4();
            DefaultBackground = Color.Black.ToVector4();
        }

        public IEnumerable<IFileHandler> GetLoadHandlers() =>
            [new SurfaceDocument(), new SurfaceFile()];

        public override string ToString() =>
            Title;
    }
}
