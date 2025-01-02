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

    public void MouseOver(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditingSurface.FontSize, Color.Green);

        if (!isActive) return;

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
        {
            document.EditingSurface.Surface.Clear(hoveredCellPosition.X, hoveredCellPosition.Y, 1);

            document.VisualLayerToolLower.Surface!.Surface[hoveredCellPosition.X - document.EditingSurface.Surface.ViewPosition.X, hoveredCellPosition.Y - document.EditingSurface.Surface.ViewPosition.Y].Background = _emptyCellColor;
            document.VisualLayerToolLower.Surface!.IsDirty = true;
        }
    }

    public void Reset(Document document) { }

    public void OnSelected(Document document)
    {
        DocumentViewChanged(document);
    }

    public void OnDeselected(Document document)
    {
        document.VisualLayerToolLower.Surface.DefaultBackground = Color.Transparent;
        document.VisualLayerToolLower.Clear();
    }

    public void DocumentViewChanged(Document document)
    {
        //document.VisualLayerToolLower.Update(document.VisualToolContainer, TimeSpan.Zero);

        document.VisualLayerToolLower.Surface.DefaultBackground = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        document.VisualLayerToolLower.Clear();

        var clearCell = new ColoredGlyph(document.EditingSurface.Surface.DefaultForeground, document.EditingSurface.Surface.DefaultBackground, 0);

        for (int index = 0; index < document.VisualLayerToolLower.Surface.Surface.Count; index++)
        {
            ColoredGlyphBase renderCell = document.EditingSurface.Surface[(Point.FromIndex(index, document.VisualLayerToolLower.Surface.Surface.Width) + document.EditingSurface.Surface.ViewPosition).ToIndex(document.EditingSurface.Surface.Width)];

            if (renderCell.Foreground == clearCell.Foreground &&
                renderCell.Background == clearCell.Background &&
                renderCell.Glyph == clearCell.Glyph)

                document.VisualLayerToolLower.Surface.Surface[index].Background = _emptyCellColor;
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
