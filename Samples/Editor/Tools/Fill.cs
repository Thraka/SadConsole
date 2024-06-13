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
        IScreenSurface surface = ImGuiCore.State.GetOpenDocument().Surface;

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

    public void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, surface.Surface.ViewPosition, surface.FontSize, Color.Green);

        if (!isActive) return;

        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            ColoredGlyph cellToMatch = new();
            ColoredGlyphBase currentFillCell = SharedToolSettings.Tip;

            surface.Surface[hoveredCellPosition].CopyAppearanceTo(cellToMatch);

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

            List<ColoredGlyphBase> cells = new List<ColoredGlyphBase>(surface.Surface);

            Func<ColoredGlyphBase, SadConsole.Algorithms.NodeConnections<ColoredGlyphBase>> getConnectedCells = (c) =>
            {
                Algorithms.NodeConnections<ColoredGlyphBase> connections = new Algorithms.NodeConnections<ColoredGlyphBase>();

                var position = Point.FromIndex(cells.IndexOf(c), surface.Surface.Width);

                connections.West = surface.Surface.IsValidCell(position.X - 1, position.Y) ? surface.Surface[position.X - 1, position.Y] : null;
                connections.East = surface.Surface.IsValidCell(position.X + 1, position.Y) ? surface.Surface[position.X + 1, position.Y] : null;
                connections.North = surface.Surface.IsValidCell(position.X, position.Y - 1) ? surface.Surface[position.X, position.Y - 1] : null;
                connections.South = surface.Surface.IsValidCell(position.X, position.Y + 1) ? surface.Surface[position.X, position.Y + 1] : null;

                return connections;
            };

            if (!isTargetCell(currentFillCell))
                SadConsole.Algorithms.FloodFill<ColoredGlyphBase>(surface.Surface[hoveredCellPosition], isTargetCell, fillCell, getConnectedCells);

            surface.Surface.IsDirty = true;
        }
        else if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
        {
            surface.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
            
            surface.IsDirty = true;
        }
    }

    public void OnSelected() { }

    public void OnDeselected() { }

    public void DocumentViewChanged() { }

    public void DrawOverDocument() { }
}
