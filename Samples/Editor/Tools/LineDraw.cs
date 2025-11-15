using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;
using Document = SadConsole.Editor.Documents.Document;

namespace SadConsole.Editor.Tools;

internal class LineDraw : ITool
{
    private class LineStyle: ITitle
    {
        public int[] Glyphs;

        public string Title { get; set; }

        public LineStyle(string title, int[] glyphs)
        {
            Title = title;
            Glyphs = glyphs;
        }
    }

    private ImGuiList<LineStyle> _lineStyles;

    public string Title => "\uf038 Line Draw";

    public string Description =>
        """
        Draws and connects lines on the surface (within the viewable area).

        Hold the left-mouse button to draw.

        The right-mouse button changes the current pencil tip to the foreground and background colors that are under the cursor.
        """;


    public LineDraw()
    {
        _lineStyles = new ImGuiList<LineStyle>(0,
            new("Thin", ICellSurface.ConnectedLineThin),
            new("Thick", ICellSurface.ConnectedLineThick),
            new("3D", ICellSurface.Connected3dBox)
            );
    }

    public void BuildSettingsPanel(Document document)
    {
        ImGui.SeparatorText(Title);
        bool supportsObjects = document is IDocumentSimpleObjects;

        ImGuiSC.BeginGroupPanel("Settings");

        ImGui.Text("Line Style:");
        ImGui.Combo("##linedraw_styles", ref _lineStyles.SelectedItemIndex, _lineStyles.Names, 3, 3);


        Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
        Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
        int glyph = SharedToolSettings.Tip.Glyph;
        ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(SharedToolSettings.Tip.Mirror);

        if (SettingsTable.BeginTable("linedrawsettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
        {

            SettingsTable.DrawCommonSettings(true, true, false, false, true,
                ref foreground, document.EditingSurface.Surface.DefaultForeground.ToVector4(),
                ref background, document.EditingSurface.Surface.DefaultBackground.ToVector4(),
                ref mirror,
                ref glyph, document.EditingSurfaceFont, ImGuiCore.Renderer);

            SettingsTable.EndTable();
        }

        SharedToolSettings.Tip.Foreground = foreground.ToColor();
        SharedToolSettings.Tip.Background = background.ToColor();

        ImGuiSC.EndGroupPanel();
    }

    bool _isDrawing = false;

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
        {
            document.EditingSurface.Surface[hoveredCellPosition].Clear();
            SharedToolSettings.Tip.CopyAppearanceTo(document.EditingSurface.Surface[hoveredCellPosition]);

            document.EditingSurface.Surface.SetGlyph(hoveredCellPosition.X, hoveredCellPosition.Y, _lineStyles.SelectedItem!.Glyphs[0]);

            document.EditingSurface.IsDirty = true;
        }
        else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right))
        {
            ColoredGlyphBase sourceCell = document.EditingSurface.Surface[hoveredCellPosition];

            SharedToolSettings.Tip.Foreground = sourceCell.Foreground;
            SharedToolSettings.Tip.Background = sourceCell.Background;
        }

        if (ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
        {
            document.EditingSurface.Surface.ConnectLines(_lineStyles.SelectedItem.Glyphs, document.EditingSurface.Surface.View);
        }
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}
