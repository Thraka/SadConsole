using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using Document = SadConsole.Editor.Documents.Document;

namespace SadConsole.Editor.Tools;

internal class Center : ITool
{
    public string Title => "\uf040 Center";

    public string Description =>
        """
        Left click to set the center of the animation.
        """;

    public void BuildSettingsPanel(Document document)
    {
    }

    private void ConfigureToolMode(Document document)
    {
        // Set a darkened semi-transparent background for the center
        document.VisualLayerToolLower.Surface.DefaultBackground = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        document.VisualLayerToolLower.Clear();

        var pos = ((DocumentAnimated)document)._baseAnimation.Center - document.EditingSurface.Surface.ViewPosition;

        if (document.VisualLayerToolLower.Surface.IsValidCell(pos))
            document.VisualLayerToolLower.Surface[pos].Glyph = '*';

        document.VisualLayerToolLower.Surface.IsDirty = true;
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        if (!isActive) return;

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
        {
            var animation = ((DocumentAnimated)document)._baseAnimation;
            animation.Center = hoveredCellPosition;
            ConfigureToolMode(document);
        }
    }

    public void OnSelected(Document document) =>
        ConfigureToolMode(document);

    public void OnDeselected(Document document) =>
        document.ResetVisualLayers();

    public void Reset(Document document) =>
        ConfigureToolMode(document);

    public void DocumentViewChanged(Document document) =>
        ConfigureToolMode(document);

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}
