using System.Numerics;
using System.Reflection.Metadata;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.Host;
using SadConsole.ImGuiSystem;
using SadConsole.UI.Controls;
using Document = SadConsole.Editor.Documents.Document;

namespace SadConsole.Editor.Tools;

internal class Pencil : ITool
{
    public string Title => "\uf040 Pencil";
    public ToolMode.Modes CurrentMode;

    public string Description =>
        """
        Draw glyphs on the surface.

        Use the left mouse button to draw.

        The right mouse button changes the current pencil tip to the foreground, background, and glyph, that is under the cursor.
        - Hold shift when right clicking to only copy the colors.
        - Hold ctrl when right clicking to only copy the glyph.
        """;

    public void BuildSettingsPanel(Document document)
    {
        ImGui.AlignTextToFramePadding();
        ImGui.Text("Mode"u8);
        ImGui.SameLine();

        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        if (ImGui.Combo("##toolmode", ref document.ToolModes.SelectedItemIndex, document.ToolModes.Names, document.ToolModes.Count))
            ConfigureToolMode(document);

        // Drawing mode
        if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Draw)
        {
            ImGuiSC.BeginGroupPanel("Settings");

            Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
            Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
            int glyph = SharedToolSettings.Tip.Glyph;
            ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(SharedToolSettings.Tip.Mirror);

            if (SettingsTable.BeginTable("pencilsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
            {
                SettingsTable.DrawCommonSettings(true, true, true, true, true,
                    ref foreground, document.EditingSurface.Surface.DefaultForeground.ToVector4(),
                    ref background, document.EditingSurface.Surface.DefaultBackground.ToVector4(),
                    ref mirror,
                    ref glyph, document.EditingSurfaceFont, ImGuiCore.Renderer);

                SettingsTable.EndTable();
            }

            SharedToolSettings.Tip.Foreground = foreground.ToColor();
            SharedToolSettings.Tip.Background = background.ToColor();
            SharedToolSettings.Tip.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(mirror);
            SharedToolSettings.Tip.Glyph = glyph;

            ImGuiSC.EndGroupPanel();
        }

        // Objects mode
        else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Objects)
        {
            if (SharedToolSettings.ImGuiDrawObjects(document, out var obj))
                SharedToolSettings.Tip = (ColoredGlyph)obj.Visual.Clone();
        }

        // Zones mode
        else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Zones)
        {
            bool debugZones = false;

            SharedToolSettings.ImGuiDrawZones(document, ref debugZones, out var obj);
        }
    }

    private void ConfigureToolMode(Document document)
    {
        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        if (CurrentMode == ToolMode.Modes.Empty)
        {
            document.VisualLayerToolLower.Surface.DefaultBackground = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            document.VisualLayerToolLower.Clear();

            var clearCell = new ColoredGlyph(document.EditingSurface.Surface.DefaultForeground, document.EditingSurface.Surface.DefaultBackground, 0);

            for (int index = 0; index < document.VisualLayerToolLower.Surface.Surface.Count; index++)
            {
                ColoredGlyphBase renderCell = document.EditingSurface.Surface[(Point.FromIndex(index, document.VisualLayerToolLower.Surface.Surface.Width) + document.EditingSurface.Surface.ViewPosition).ToIndex(document.EditingSurface.Surface.Width)];

                if (renderCell.Foreground == clearCell.Foreground &&
                    renderCell.Background == clearCell.Background &&
                    renderCell.Glyph == clearCell.Glyph)

                    document.VisualLayerToolLower.Surface.Surface[index].Background = Core.Settings.EmptyCellColor;
            }
        }
        else
        {
            // Reset empty tool mode
            document.VisualLayerToolLower.Surface.DefaultBackground = Color.Transparent;
            document.VisualLayerToolLower.Clear();
        }
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

        if (!isActive) return;

        if (CurrentMode == ToolMode.Modes.Draw || CurrentMode == ToolMode.Modes.Objects)
        {
            if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
            {
                document.EditingSurface.Surface[hoveredCellPosition].Clear();
                SharedToolSettings.Tip.CopyAppearanceTo(document.EditingSurface.Surface[hoveredCellPosition]);
                document.EditingSurface.IsDirty = true;
            }
            else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right) && CurrentMode == ToolMode.Modes.Draw)
            {
                ColoredGlyphBase sourceCell = document.EditingSurface.Surface[hoveredCellPosition];

                if (ImGuiP.IsKeyDown(ImGuiKey.ModShift))
                {
                    SharedToolSettings.Tip.Foreground = sourceCell.Foreground;
                    SharedToolSettings.Tip.Background = sourceCell.Background;
                }
                else if (ImGuiP.IsKeyDown(ImGuiKey.ModCtrl))
                {
                    SharedToolSettings.Tip.Glyph = sourceCell.Glyph;
                }
                else
                    document.EditingSurface.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
            }
        }
        else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Empty)
        {
            if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
            {
                document.EditingSurface.Surface.Clear(hoveredCellPosition.X, hoveredCellPosition.Y, 1);

                document.VisualLayerToolLower.Surface!.Surface[hoveredCellPosition.X - document.EditingSurface.Surface.ViewPosition.X,
                                                                hoveredCellPosition.Y - document.EditingSurface.Surface.ViewPosition.Y]
                                                               .Background = Core.Settings.EmptyCellColor;

                document.VisualLayerToolLower.Surface.IsDirty = true;
            }
        }

        // Zones
        else
        {
        }
    }

    public void OnSelected(Document document) =>
        ConfigureToolMode(document);

    public void OnDeselected(Document document) =>
        document.ResetVisualLayers();

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) =>
        ConfigureToolMode(document);

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}
