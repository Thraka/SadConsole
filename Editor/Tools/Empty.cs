using Hexa.NET.ImGui;
using SadConsole.Components;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Empty : ITool
{
    private Color _emptyCellColor = Color.NavajoWhite;
    private nint _gridImage = -1;
    private Overlay _toolOverlay = new();

    public string Title => "\uf2d3 Empty Cells";

    public string Description => "Empties existing cells.";

    public void BuildSettingsPanel(Document document)
    {
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        if (!isActive) return;

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
        {
            document.EditingSurface.Surface.Clear(hoveredCellPosition.X, hoveredCellPosition.Y, 1);

            document.VisualLayerToolMiddle.Surface!.Surface[hoveredCellPosition.X - document.EditingSurface.Surface.ViewPosition.X, hoveredCellPosition.Y - document.EditingSurface.Surface.ViewPosition.Y].Background = _emptyCellColor;
            document.VisualLayerToolMiddle.Surface!.IsDirty = true;
        }
    }

    public void Reset(Document document) { }

    public void OnSelected(Document document)
    {
        DocumentViewChanged(document);
    }

    public void OnDeselected(Document document)
    {
        document.VisualLayerToolMiddle.Surface.DefaultBackground = Color.Transparent;
        document.VisualLayerToolMiddle.Clear();
    }

    public void DocumentViewChanged(Document document)
    {
        document.VisualLayerToolMiddle.Surface.DefaultBackground = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        document.VisualLayerToolMiddle.Clear();

        var clearCell = new ColoredGlyph(document.EditingSurface.Surface.DefaultForeground, document.EditingSurface.Surface.DefaultBackground, 0);

        for (int index = 0; index < document.VisualLayerToolMiddle.Surface.Surface.Count; index++)
        {
            ColoredGlyphBase renderCell = document.EditingSurface.Surface[(Point.FromIndex(index, document.VisualLayerToolMiddle.Surface.Surface.Width) + document.EditingSurface.Surface.ViewPosition).ToIndex(document.EditingSurface.Surface.Width)];

            if (renderCell.Foreground == clearCell.Foreground &&
                renderCell.Background == clearCell.Background &&
                renderCell.Glyph == clearCell.Glyph)

                document.VisualLayerToolMiddle.Surface.Surface[index].Background = _emptyCellColor;
        }

        //Overlay.Surface.Render(TimeSpan.Zero);
        //_gridImage = ImGuiCore.Renderer.BindTexture(((Host.GameTexture)_gridSurface.Renderer!.Output).Texture);
    }

    public void DrawOverDocument(Document document)
    {
        //Vector2 min = ImGui.GetItemRectMin();
        //Vector2 max = ImGui.GetItemRectMax();
        //ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        //drawList.AddImage(_gridImage, min, max);
    }

    public override string ToString() =>
        Title;
}
