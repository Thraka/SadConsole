using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.Editor.FileHandlers;
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
            ImGui.Text("Name");
            ImGui.InputText("##name", ref Name, 50);

            ImGui.SeparatorText("Slice Grid");

            // Colors for each slice role
            uint cornerColor = ImGui.GetColorU32(new Vector4(0.30f, 0.30f, 0.50f, 1.0f));
            uint edgeColor = ImGui.GetColorU32(new Vector4(0.20f, 0.45f, 0.30f, 1.0f));
            uint centerColor = ImGui.GetColorU32(new Vector4(0.50f, 0.30f, 0.20f, 1.0f));

            // 4x4 table: [height inputs] [left col] [center col] [right col]
            //             [width inputs across top row]
            if (ImGui.BeginTable("slice_grid", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingStretchSame))
            {
                ImGui.TableSetupColumn("##heights", ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("H: 000").X + ImGui.GetStyle().FramePadding.X * 2);
                ImGui.TableSetupColumn("##left");
                ImGui.TableSetupColumn("##center");
                ImGui.TableSetupColumn("##right");

                // Row 0: header — empty corner + column width inputs
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);

                ImGui.TableSetColumnIndex(1);
                ImGui.SetNextItemWidth(-1);
                ImGui.DragInt("##lw", ref LeftWidth, 1, 1, 100, "W: %d");

                ImGui.TableSetColumnIndex(2);
                ImGui.SetNextItemWidth(-1);
                ImGui.DragInt("##cw", ref CenterWidth, 1, 1, 100, "W: %d");

                ImGui.TableSetColumnIndex(3);
                ImGui.SetNextItemWidth(-1);
                ImGui.DragInt("##rw", ref RightWidth, 1, 1, 100, "W: %d");

                // Row 1: Top
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.SetNextItemWidth(-1);
                ImGui.DragInt("##th", ref TopHeight, 1, 1, 100, "H: %d");

                DrawSliceCell(1, "TL", cornerColor);
                DrawSliceCell(2, "Top", edgeColor);
                DrawSliceCell(3, "TR", cornerColor);

                // Row 2: Center
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.SetNextItemWidth(-1);
                ImGui.DragInt("##ch", ref CenterHeight, 1, 1, 100, "H: %d");

                DrawSliceCell(1, "Left", edgeColor);
                DrawSliceCell(2, "Center", centerColor);
                DrawSliceCell(3, "Right", edgeColor);

                // Row 3: Bottom
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.SetNextItemWidth(-1);
                ImGui.DragInt("##bh", ref BottomHeight, 1, 1, 100, "H: %d");

                DrawSliceCell(1, "BL", cornerColor);
                DrawSliceCell(2, "Bottom", edgeColor);
                DrawSliceCell(3, "BR", cornerColor);

                ImGui.EndTable();
            }

            // Clamp all values to valid range
            LeftWidth = Math.Max(1, LeftWidth);
            CenterWidth = Math.Max(1, CenterWidth);
            RightWidth = Math.Max(1, RightWidth);
            TopHeight = Math.Max(1, TopHeight);
            CenterHeight = Math.Max(1, CenterHeight);
            BottomHeight = Math.Max(1, BottomHeight);

            int totalWidth = LeftWidth + CenterWidth + RightWidth;
            int totalHeight = TopHeight + CenterHeight + BottomHeight;
            ImGui.Text($"Surface Size: {totalWidth} x {totalHeight}");

            ImGui.Separator();

            if (SettingsTable.BeginTable("new_doc_colors"))
            {
                SettingsTable.DrawColor("Def. Foreground", "##fore", ref DefaultForeground, Color.White.ToVector4(), true, out _);
                SettingsTable.DrawColor("Def. Background", "##back", ref DefaultBackground, Color.Black.ToVector4(), true, out _);

                SettingsTable.EndTable();
            }
        }

        private static void DrawSliceCell(int columnIndex, string label, uint bgColor)
        {
            ImGui.TableSetColumnIndex(columnIndex);
            ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, bgColor);
            ImGui.AlignTextToFramePadding();
            ImGui.Text(label);
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

            return new SlicedDocument(border) { Title = Name };
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
            [new SlicedDocumentFile()];

        public override string ToString() =>
            Title;
    }
}
