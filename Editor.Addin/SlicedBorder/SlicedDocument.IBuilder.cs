using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.Editor.FileHandlers;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;
using SadRogue.Primitives;

namespace SadConsole.Editor.Addin.SlicedBorder;

public partial class SlicedDocument: Document
{
    public class Builder : IBuilder
    {
        public string Name;

        // Column widths: left, center, right
        public int LeftWidth;
        public int CenterWidth;
        public int RightWidth;

        // Row heights: top, center, bottom
        public int TopHeight;
        public int CenterHeight;
        public int BottomHeight;

        public Vector4 DefaultForeground;
        public Vector4 DefaultBackground;

        public string Title => "Sliced Box";

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
                // Column widths
                SettingsTable.DrawInt("Left Width", "##leftwidth", ref LeftWidth, 1, 100);
                SettingsTable.DrawInt("Center Width", "##centerwidth", ref CenterWidth, 1, 100);
                SettingsTable.DrawInt("Right Width", "##rightwidth", ref RightWidth, 1, 100);

                // Row heights
                SettingsTable.DrawInt("Top Height", "##topheight", ref TopHeight, 1, 100);
                SettingsTable.DrawInt("Center Height", "##centerheight", ref CenterHeight, 1, 100);
                SettingsTable.DrawInt("Bottom Height", "##bottomheight", ref BottomHeight, 1, 100);

                // Computed surface size (read-only)
                int totalWidth = LeftWidth + CenterWidth + RightWidth;
                int totalHeight = TopHeight + CenterHeight + BottomHeight;
                SettingsTable.DrawText("Surface Size", $"{totalWidth} x {totalHeight}");

                SettingsTable.DrawColor("Def. Foreground", "##fore", ref DefaultForeground, Color.White.ToVector4(), true, out _);
                SettingsTable.DrawColor("Def. Background", "##back", ref DefaultBackground, Color.Black.ToVector4(), true, out _);

                SettingsTable.EndTable();
            }
        }

        public bool IsDocumentValid()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && LeftWidth > 0 && CenterWidth > 0 && RightWidth > 0
                && TopHeight > 0 && CenterHeight > 0 && BottomHeight > 0;
        }

        public Document CreateDocument()
        {
            int totalWidth = LeftWidth + CenterWidth + RightWidth;
            int totalHeight = TopHeight + CenterHeight + BottomHeight;

            CellSurface surface = new(totalWidth, totalHeight);
            surface.DefaultForeground = DefaultForeground.ToColor();
            surface.DefaultBackground = DefaultBackground.ToColor();
            surface.Clear();

            global::SadConsole.SlicedBorder border = new(surface,
                new Rectangle(0, 0, LeftWidth, TopHeight),
                new Rectangle(LeftWidth, 0, CenterWidth, TopHeight),
                new Rectangle(LeftWidth + CenterWidth, 0, RightWidth, TopHeight),
                new Rectangle(0, TopHeight, LeftWidth, CenterHeight),
                new Rectangle(LeftWidth, TopHeight, CenterWidth, CenterHeight),
                new Rectangle(LeftWidth + CenterWidth, TopHeight, RightWidth, CenterHeight),
                new Rectangle(0, TopHeight + CenterHeight, LeftWidth, BottomHeight),
                new Rectangle(LeftWidth, TopHeight + CenterHeight, CenterWidth, BottomHeight),
                new Rectangle(LeftWidth + CenterWidth, TopHeight + CenterHeight, RightWidth, BottomHeight)
            );

            return new SlicedDocument(surface, border) { Title = Name };
        }

        public void ResetBuilder()
        {
            Name = Document.GenerateName("SlicedBox");
            LeftWidth = 1;
            CenterWidth = 1;
            RightWidth = 1;
            TopHeight = 1;
            CenterHeight = 1;
            BottomHeight = 1;
            DefaultForeground = Color.White.ToVector4();
            DefaultBackground = Color.Black.ToVector4();
        }

        public IEnumerable<IFileHandler> GetLoadHandlers() =>
            [];

        public override string ToString() =>
            Title;
    }
}
