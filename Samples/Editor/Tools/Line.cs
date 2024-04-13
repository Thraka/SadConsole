using System.Numerics;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Line : ITool, IOverlay
{
    private bool _isDrawing = false;
    private bool _isFirstPointSelected = false;
    private Point _firstPoint;
    private Point _secondPoint;
    private Overlay _toolOverlay = new();
    private bool _isCancelled;

    public string Name => "Line";

    public string Description => """
        Draws a line.

        The line can be set to use a connected glyph.

        Depress the left mouse button to start drawing. Hold down the button and drag the mouse to draw the line. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public Overlay Overlay => _toolOverlay;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGuiWidgets.BeginGroupPanel("Settings");

        Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
        Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
        Mirror mirror = SharedToolSettings.Tip.Mirror;
        int glyph = SharedToolSettings.Tip.Glyph;
        IScreenSurface surface = ((IDocumentSurface)ImGuiCore.State.GetOpenDocument()).Surface;

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

        GuiParts.Tools.ToolHelpers.HighlightCell(hoveredCellPosition, surface.Surface.ViewPosition, surface.FontSize, Color.Green);

        if (!_isDrawing)
        {
            // Cancelled but left mouse finally released, exit cancelled
            if (_isCancelled && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                _isCancelled = false;

            // Cancelled
            if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && (ImGui.IsMouseClicked(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape)))
            {
                OnDeselected();
                _isCancelled = true;
                Overlay.Surface.Clear();
            }

            if (_isCancelled)
                return;

            if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && isActive)
            {
                if (!_isFirstPointSelected)
                {
                    _isFirstPointSelected = true;

                    _firstPoint = hoveredCellPosition - surface.Surface.ViewPosition;
                }

                _secondPoint = hoveredCellPosition - surface.Surface.ViewPosition;

                Overlay.Surface.Clear();
                Overlay.Surface.DrawLine(_firstPoint,
                                         _secondPoint,
                                         SharedToolSettings.Tip.Glyph,
                                         SharedToolSettings.Tip.Foreground,
                                         SharedToolSettings.Tip.Background,
                                         SharedToolSettings.Tip.Mirror);
            }
            else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (_firstPoint != Point.None)
                {
                    surface.Surface.DrawLine(_firstPoint + surface.Surface.ViewPosition,
                                             _secondPoint + surface.Surface.ViewPosition,
                                             SharedToolSettings.Tip.Glyph,
                                             SharedToolSettings.Tip.Foreground,
                                             SharedToolSettings.Tip.Background,
                                             SharedToolSettings.Tip.Mirror);


                    Overlay.Surface.Clear();
                }

                OnDeselected();
            }
        }
    }

    public void OnSelected()
    {

    }

    public void OnDeselected()
    {
        _isCancelled = false;
        _isDrawing = false;
        _firstPoint = Point.None;
        _isFirstPointSelected = false;
    }

    public void DocumentViewChanged() { }

    public void DrawOverDocument() { }
}
