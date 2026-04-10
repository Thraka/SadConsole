using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.Editor.FileHandlers;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem.Rendering;
using SadRogue.Primitives;

namespace SadConsole.Editor.Addin.SlicedBorder;

/*
 * TODO:
 * 1. Core object in SadConsole needs to have 4 colors that are used in the surface. They can be customized per border draw.
 * 2. Name support in the editor settings. This isn't persisted in the only file handler (which is the raw object) so we need a new file handler demoing serializing of the document.
 */


public partial class SlicedDocument : Document
{
    public SadConsole.SlicedBorder Border { get; set; }

    private Builder? _resizeBuilder;

    public SlicedDocument(SadConsole.SlicedBorder border)
    {
        Border = border;

        ScreenSurface screenSurface = new(border.Surface);
        EditingSurface = screenSurface;
        EditingSurfaceFont = (SadFont)Game.Instance.DefaultFont;
        EditingSurfaceFontSize = EditingSurfaceFont.GetFontSize(IFont.Sizes.One);
        EditorFontSize = EditingSurfaceFontSize;

        // Scene draws itself with custom rendering
        Options.UseToolsWindow = true;
        Options.DisableScrolling = false;
    }

    public override void BuildUiDocumentSettings(ImGuiRenderer renderer)
    {
        ImGui.SeparatorText("Sliced Border Settings"u8);

        // Read-only visual grid showing current border layout
        {
            uint cornerColor = ImGui.GetColorU32(new Vector4(0.30f, 0.30f, 0.50f, 1.0f));
            uint edgeColor = ImGui.GetColorU32(new Vector4(0.20f, 0.45f, 0.30f, 1.0f));
            uint centerColor = ImGui.GetColorU32(new Vector4(0.50f, 0.30f, 0.20f, 1.0f));

            if (ImGui.BeginTable("slice_grid_readonly", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingStretchSame))
            {
                ImGui.TableSetupColumn("##heights", ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("H: 000").X + ImGui.GetStyle().FramePadding.X * 2);
                ImGui.TableSetupColumn("##left");
                ImGui.TableSetupColumn("##center");
                ImGui.TableSetupColumn("##right");

                // Row 0: column width headers
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.TableSetColumnIndex(1);
                ImGui.Text($"W: {Border.TopLeft.Width}");
                ImGui.TableSetColumnIndex(2);
                ImGui.Text($"W: {Border.Top.Width}");
                ImGui.TableSetColumnIndex(3);
                ImGui.Text($"W: {Border.TopRight.Width}");

                // Row 1: Top
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"H: {Border.TopLeft.Height}");
                DrawSliceCell(1, "TL", cornerColor);
                DrawSliceCell(2, "Top", edgeColor);
                DrawSliceCell(3, "TR", cornerColor);

                // Row 2: Center
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"H: {Border.Left.Height}");
                DrawSliceCell(1, "Left", edgeColor);
                DrawSliceCell(2, "Center", centerColor);
                DrawSliceCell(3, "Right", edgeColor);

                // Row 3: Bottom
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"H: {Border.BottomLeft.Height}");
                DrawSliceCell(1, "BL", cornerColor);
                DrawSliceCell(2, "Bottom", edgeColor);
                DrawSliceCell(3, "BR", cornerColor);

                ImGui.EndTable();
            }

            ImGui.Text($"Surface Size: {Border.Surface.Width} x {Border.Surface.Height}");
        }

        // Resize button and popup
        if (ImGui.Button("Resize"))
        {
            _resizeBuilder = new Builder
            {
                Name = Title,
                LeftWidth = Border.TopLeft.Width,
                CenterWidth = Border.Top.Width,
                RightWidth = Border.TopRight.Width,
                TopHeight = Border.TopLeft.Height,
                CenterHeight = Border.Left.Height,
                BottomHeight = Border.BottomLeft.Height,
                DefaultForeground = Border.Surface.DefaultForeground.ToVector4(),
                DefaultBackground = Border.Surface.DefaultBackground.ToVector4(),
            };
            ImGui.OpenPopup("Resize Sliced Border");
        }

        ImGuiSC.CenterNextWindowOnAppearing(new Vector2(Core.Settings.WindowNewDocWidthFactor * ImGui.GetFontSize(), -1));

        if (ImGui.BeginPopupModal("Resize Sliced Border"u8))
        {
            _resizeBuilder!.ImGuiNewDocument(renderer);

            if (ImGuiSC.WindowDrawButtons(out bool accepted, !_resizeBuilder.IsDocumentValid()))
            {
                if (accepted)
                    ApplyResize(_resizeBuilder);

                _resizeBuilder = null;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        ImGui.SeparatorText("Editing Surface Font"u8);

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Font: "u8);
        ImGui.SameLine();
        if (ImGui.Button($"{EditingSurfaceFont.Name} | {EditingSurfaceFontSize}"))
            FontSelectionWindow.Show(renderer, EditingSurfaceFont, EditingSurfaceFontSize, (font, fontSize) =>
            {
                EditingSurfaceFont = (SadFont)font;
                EditingSurfaceFontSize = fontSize;
                EditorFontSize = fontSize;
                EditingSurface.Font = EditingSurfaceFont;
                EditingSurface.FontSize = EditorFontSize;
                EditingSurface.IsDirty = true;
                VisualTool.Font = EditingSurfaceFont;
                VisualTool.IsDirty = true;
            });

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Font Size: "u8);
        ImGui.SameLine();
        if (ImGui.Button(EditorFontSize.ToString()))
        {
            ImGui.OpenPopup("editorfontsize_select");
        }
        ImGui.SameLine();
        if (ImGui.Button("Reset"u8))
        {
            EditorFontSize = EditingSurfaceFontSize;

            // Force overlay to update and match surface
            //ComposeVisual();
        }

        if (FontSizePopup.Show("editorfontsize_select", EditingSurfaceFont, ref EditorFontSize))
        {
            // Force overlay to update and match surface
            EditingSurface.IsDirty = true;
            VisualTool.IsDirty = true;
        }
    }

    public override void ImGuiDrawSurfaceTextureAfter(ImGuiRenderer renderer, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 texturePos = ImGui.GetItemRectMin();
        Vector2 textureEnd = ImGui.GetItemRectMax();

        Point viewPos = EditingSurface.Surface.ViewPosition;

        uint outlineColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 1f, 0f, 0.3f));
        uint labelBgColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.1f, 0.3f, 0.8f, 0.85f));
        uint labelTextColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f));

        DrawRegion(Border.TopLeft, "TopLeft");
        DrawRegion(Border.Top, "Top");
        DrawRegion(Border.TopRight, "TopRight");
        DrawRegion(Border.Left, "Left");
        DrawRegion(Border.Center, "Center");
        DrawRegion(Border.Right, "Right");
        DrawRegion(Border.BottomLeft, "BotLeft");
        DrawRegion(Border.Bottom, "Bottom");
        DrawRegion(Border.BottomRight, "BotRight");

        void DrawRegion(Rectangle region, string name)
        {
            float pixelX = texturePos.X + (region.Position.X - viewPos.X) * EditorFontSize.X;
            float pixelY = texturePos.Y + (region.Position.Y - viewPos.Y) * EditorFontSize.Y;
            float pixelW = region.Width * EditorFontSize.X;
            float pixelH = region.Height * EditorFontSize.Y;

            Vector2 min = new(pixelX, pixelY);
            Vector2 max = new(pixelX + pixelW, pixelY + pixelH);

            // Clip to visible texture area
            Vector2 clippedMin = Vector2.Max(min, texturePos);
            Vector2 clippedMax = Vector2.Min(max, textureEnd);

            if (clippedMin.X >= clippedMax.X || clippedMin.Y >= clippedMax.Y)
                return;

            // Draw region outline
            drawList.AddRect(clippedMin, clippedMax, outlineColor, 0f, (ImDrawFlags)0, 2.0f);
        }
    }

    public override IEnumerable<IFileHandler> GetSaveHandlers() =>
        [new SlicedDocumentFile()];

    private void ApplyResize(Builder builder)
    {
        int newTotalWidth = builder.LeftWidth + builder.CenterWidth + builder.RightWidth;
        int newTotalHeight = builder.TopHeight + builder.CenterHeight + builder.BottomHeight;

        CellSurface newSurface = new(newTotalWidth, newTotalHeight);
        newSurface.DefaultForeground = builder.DefaultForeground.ToColor();
        newSurface.DefaultBackground = builder.DefaultBackground.ToColor();
        newSurface.Clear();

        // Calculate new rectangles
        Rectangle newTopLeft = new(0, 0, builder.LeftWidth, builder.TopHeight);
        Rectangle newTop = new(builder.LeftWidth, 0, builder.CenterWidth, builder.TopHeight);
        Rectangle newTopRight = new(builder.LeftWidth + builder.CenterWidth, 0, builder.RightWidth, builder.TopHeight);
        Rectangle newLeft = new(0, builder.TopHeight, builder.LeftWidth, builder.CenterHeight);
        Rectangle newCenter = new(builder.LeftWidth, builder.TopHeight, builder.CenterWidth, builder.CenterHeight);
        Rectangle newRight = new(builder.LeftWidth + builder.CenterWidth, builder.TopHeight, builder.RightWidth, builder.CenterHeight);
        Rectangle newBottomLeft = new(0, builder.TopHeight + builder.CenterHeight, builder.LeftWidth, builder.BottomHeight);
        Rectangle newBottom = new(builder.LeftWidth, builder.TopHeight + builder.CenterHeight, builder.CenterWidth, builder.BottomHeight);
        Rectangle newBottomRight = new(builder.LeftWidth + builder.CenterWidth, builder.TopHeight + builder.CenterHeight, builder.RightWidth, builder.BottomHeight);

        // Copy old region data into new regions, preserving existing surface art
        CopyRegion(Border.Surface, Border.TopLeft, newSurface, newTopLeft);
        CopyRegion(Border.Surface, Border.Top, newSurface, newTop);
        CopyRegion(Border.Surface, Border.TopRight, newSurface, newTopRight);
        CopyRegion(Border.Surface, Border.Left, newSurface, newLeft);
        CopyRegion(Border.Surface, Border.Center, newSurface, newCenter);
        CopyRegion(Border.Surface, Border.Right, newSurface, newRight);
        CopyRegion(Border.Surface, Border.BottomLeft, newSurface, newBottomLeft);
        CopyRegion(Border.Surface, Border.Bottom, newSurface, newBottom);
        CopyRegion(Border.Surface, Border.BottomRight, newSurface, newBottomRight);

        SadConsole.SlicedBorder newBorder = new(newSurface,
            newTopLeft, newTop, newTopRight,
            newLeft, newCenter, newRight,
            newBottomLeft, newBottom, newBottomRight);

        Border = newBorder;
        EditingSurface = new ScreenSurface(newSurface);
        EditingSurface.Font = EditingSurfaceFont;
        EditingSurface.FontSize = EditorFontSize;
        EditingSurface.IsDirty = true;
        Redraw(true, true);
    }

    private static void CopyRegion(ICellSurface source, Rectangle sourceRect, ICellSurface dest, Rectangle destRect)
    {
        int copyWidth = Math.Min(sourceRect.Width, destRect.Width);
        int copyHeight = Math.Min(sourceRect.Height, destRect.Height);

        for (int y = 0; y < copyHeight; y++)
        {
            for (int x = 0; x < copyWidth; x++)
            {
                int srcX = sourceRect.Position.X + x;
                int srcY = sourceRect.Position.Y + y;
                int dstX = destRect.Position.X + x;
                int dstY = destRect.Position.Y + y;

                if (srcX >= 0 && srcX < source.Width && srcY >= 0 && srcY < source.Height &&
                    dstX >= 0 && dstX < dest.Width && dstY >= 0 && dstY < dest.Height)
                {
                    int srcIdx = srcY * source.Width + srcX;
                    int dstIdx = dstY * dest.Width + dstX;
                    source[srcIdx].CopyAppearanceTo(dest[dstIdx]);
                }
            }
        }
    }

    private static void DrawSliceCell(int columnIndex, string label, uint bgColor)
    {
        ImGui.TableSetColumnIndex(columnIndex);
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, bgColor);
        ImGui.Text(label);
    }
}
