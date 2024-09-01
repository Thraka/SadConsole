using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Empty : ITool
{
    private Color _emptyCellColor = Color.NavajoWhite;
    private nint _gridImage = -1;
    private Overlay _toolOverlay = new();

    public string Name => "Empty Cells";

    public string Description => "Empties existing cells.";

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        ToolHelpers.HighlightCell(hoveredCellPosition, document.VisualDocument.Surface.ViewPosition, document.VisualDocument.FontSize, Color.Green);

        if (!isActive) return;

        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            document.VisualDocument.Surface.Clear(hoveredCellPosition.X, hoveredCellPosition.Y, 1);

            document.VisualToolLayerLower.Surface!.Surface[hoveredCellPosition.X - document.VisualDocument.Surface.ViewPosition.X, hoveredCellPosition.Y - document.VisualDocument.Surface.ViewPosition.Y].Background = _emptyCellColor;
            document.VisualToolLayerLower.Surface!.IsDirty = true;
        }
    }

    public void OnSelected()
    {
        DocumentViewChanged(ImGuiCore.State.GetOpenDocument());
    }

    public void OnDeselected()
    {
        Document document = ImGuiCore.State.GetOpenDocument();
        document.VisualToolLayerLower.Surface.DefaultBackground = Color.Transparent;
        document.VisualToolLayerLower.Clear();
    }

    public void DocumentViewChanged(Document document)
    {
        //document.VisualToolLayerLower.Update(document.VisualToolContainer, TimeSpan.Zero);

        document.VisualToolLayerLower.Surface.DefaultBackground = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        document.VisualToolLayerLower.Clear();

        var clearCell = new ColoredGlyph(document.VisualDocument.Surface.DefaultForeground, document.VisualDocument.Surface.DefaultBackground, 0);

        for (int index = 0; index < document.VisualToolLayerLower.Surface.Surface.Count; index++)
        {
            ColoredGlyphBase renderCell = document.VisualDocument.Surface[(Point.FromIndex(index, document.VisualToolLayerLower.Surface.Surface.Width) + document.VisualDocument.Surface.ViewPosition).ToIndex(document.VisualDocument.Surface.Width)];

            if (renderCell.Foreground == clearCell.Foreground &&
                renderCell.Background == clearCell.Background &&
                renderCell.Glyph == clearCell.Glyph)

                document.VisualToolLayerLower.Surface.Surface[index].Background = _emptyCellColor;
        }

        //Overlay.Surface.Render(TimeSpan.Zero);
        //_gridImage = ImGuiCore.Renderer.BindTexture(((Host.GameTexture)_gridSurface.Renderer!.Output).Texture);
    }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer)
    {
        //Vector2 min = ImGui.GetItemRectMin();
        //Vector2 max = ImGui.GetItemRectMax();
        //ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        //drawList.AddImage(_gridImage, min, max);
    }
}
