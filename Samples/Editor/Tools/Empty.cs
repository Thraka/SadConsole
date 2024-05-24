using System;
using System.Numerics;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Empty : ITool, IOverlay
{
    private Color _emptyCellColor = Color.NavajoWhite;
    private nint _gridImage = -1;
    private Overlay _toolOverlay = new();

    public string Name => "Empty Cells";

    public Overlay Overlay => _toolOverlay;

    public string Description => "Empties existing cells.";

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
    }

    public void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        ToolHelpers.HighlightCell(hoveredCellPosition, surface.Surface.ViewPosition, surface.FontSize, Color.Green);

        if (!isActive) return;

        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            surface.Surface[hoveredCellPosition].Clear();
            surface.Surface[hoveredCellPosition].Glyph = 0;
            surface.Surface.IsDirty = true;

            Overlay.Surface.Surface[hoveredCellPosition.X - surface.Surface.ViewPosition.X, hoveredCellPosition.Y - surface.Surface.ViewPosition.Y].Background = _emptyCellColor;
            Overlay.Surface.IsDirty = true;
        }
    }

    public void OnSelected()
    {
        DocumentViewChanged();
    }

    public void OnDeselected() { }

    public void DocumentViewChanged()
    {
        IScreenSurface surface = ImGuiCore.State.GetOpenDocument().Surface;

        Overlay.Update(surface, TimeSpan.Zero);

        Overlay.Surface.Surface.DefaultBackground = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        Overlay.Surface.Surface.Clear();

        var clearCell = new ColoredGlyph(surface.Surface.DefaultForeground, surface.Surface.DefaultBackground, 0);

        for (int index = 0; index < Overlay.Surface.Surface.Count; index++)
        {
            ColoredGlyphBase renderCell = surface.Surface[(Point.FromIndex(index, Overlay.Surface.Surface.Width) + surface.Surface.ViewPosition).ToIndex(surface.Surface.Width)];

            if (renderCell.Foreground == clearCell.Foreground &&
                renderCell.Background == clearCell.Background &&
                renderCell.Glyph == clearCell.Glyph)

                Overlay.Surface.Surface[index].Background = _emptyCellColor;
        }

        //Overlay.Surface.Render(TimeSpan.Zero);
        //_gridImage = ImGuiCore.Renderer.BindTexture(((Host.GameTexture)_gridSurface.Renderer!.Output).Texture);
    }

    public void DrawOverDocument()
    {
        //Vector2 min = ImGui.GetItemRectMin();
        //Vector2 max = ImGui.GetItemRectMax();
        //ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        //drawList.AddImage(_gridImage, min, max);
    }
}
