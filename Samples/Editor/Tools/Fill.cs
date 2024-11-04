using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Fill : ITool
{
    public string Name => "Fill";

    public string Description => """
        Fills an area of the surface.

        Use the left-mouse button to fill.

        The right-mouse button changes the current fill tip to the foreground, background, and glyph, that is under the cursor.
        """;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGuiWidgets.BeginGroupPanel("Settings");

        Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
        Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
        Mirror mirror = SharedToolSettings.Tip.Mirror;
        int glyph = SharedToolSettings.Tip.Glyph;
        IScreenSurface surface = ImGuiCore.State.GetOpenDocument().VisualDocument;

        SettingsTable.DrawCommonSettings("fillsettings", true, true, true, true, true,
                                 ref foreground, surface.Surface.DefaultForeground.ToVector4(),
                                 ref background, surface.Surface.DefaultBackground.ToVector4(),
                                 ref mirror,
                                 ref glyph, surface.Font, renderer);

        SharedToolSettings.Tip.Foreground = foreground.ToColor();
        SharedToolSettings.Tip.Background = background.ToColor();
        SharedToolSettings.Tip.Mirror = mirror;
        SharedToolSettings.Tip.Glyph = glyph;

        ImGuiWidgets.EndGroupPanel();
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.VisualDocument.Surface.ViewPosition, document.VisualDocument.FontSize, Color.Green);

        if (!isActive) return;

        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            ColoredGlyph cellToMatch = new();
            ColoredGlyphBase currentFillCell = SharedToolSettings.Tip;

            document.VisualDocument.Surface[hoveredCellPosition].CopyAppearanceTo(cellToMatch);

            Func<ColoredGlyphBase, bool> isTargetCell = (c) =>
            {
                if (c.Glyph == 0 && cellToMatch.Glyph == 0)
                    return c.Background == cellToMatch.Background;

                return c.Foreground == cellToMatch.Foreground &&
                       c.Background == cellToMatch.Background &&
                       c.Glyph == cellToMatch.Glyph &&
                       c.Mirror == cellToMatch.Mirror;
            };

            Action<ColoredGlyphBase> fillCell = (c) =>
            {
                currentFillCell.CopyAppearanceTo(c);
                //console.TextSurface.SetEffect(c, _currentFillCell.Effect);
            };

            List<ColoredGlyphBase> cells = new List<ColoredGlyphBase>(document.VisualDocument.Surface);

            Func<ColoredGlyphBase, SadConsole.Algorithms.NodeConnections<ColoredGlyphBase>> getConnectedCells = (c) =>
            {
                Algorithms.NodeConnections<ColoredGlyphBase> connections = new Algorithms.NodeConnections<ColoredGlyphBase>();

                var position = Point.FromIndex(cells.IndexOf(c), document.VisualDocument.Surface.Width);

                connections.West = document.VisualDocument.Surface.IsValidCell(position.X - 1, position.Y) ? document.VisualDocument.Surface[position.X - 1, position.Y] : null;
                connections.East = document.VisualDocument.Surface.IsValidCell(position.X + 1, position.Y) ? document.VisualDocument.Surface[position.X + 1, position.Y] : null;
                connections.North = document.VisualDocument.Surface.IsValidCell(position.X, position.Y - 1) ? document.VisualDocument.Surface[position.X, position.Y - 1] : null;
                connections.South = document.VisualDocument.Surface.IsValidCell(position.X, position.Y + 1) ? document.VisualDocument.Surface[position.X, position.Y + 1] : null;

                return connections;
            };

            if (!isTargetCell(currentFillCell))
                SadConsole.Algorithms.FloodFill<ColoredGlyphBase>(document.VisualDocument.Surface[hoveredCellPosition], isTargetCell, fillCell, getConnectedCells);

            document.VisualDocument.Surface.IsDirty = true;
        }
        else if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
        {
            document.VisualDocument.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
            
            document.VisualDocument.IsDirty = true;
        }
    }

    public void OnSelected() { }

    public void OnDeselected() { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer) { }
}
