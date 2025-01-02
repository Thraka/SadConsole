using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Line : ITool
{
    private bool _isDrawing = false;
    private bool _isFirstPointSelected = false;
    private Point _firstPoint;
    private Point _secondPoint;
    private bool _isCancelled;

    public string Title => "\ue216 Line";

    public string Description => """
        Draws a line.

        The line can be set to use a connected glyph.

        Depress the left mouse button to start drawing. Hold down the button and drag the mouse to draw the line. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public void BuildSettingsPanel(Document document)
    {
        ImGuiSC.BeginGroupPanel("Settings");

        Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
        Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
        ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(SharedToolSettings.Tip.Mirror);
        int glyph = SharedToolSettings.Tip.Glyph;
        IScreenSurface surface = document.EditingSurface;

        if (SettingsTable.BeginTable("linesettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
        {
            SettingsTable.DrawCommonSettings(true, true, true, true, true,
                ref foreground, surface.Surface.DefaultForeground.ToVector4(),
                ref background, surface.Surface.DefaultBackground.ToVector4(),
                ref mirror,
                ref glyph, surface.Font, ImGuiCore.Renderer);
            SettingsTable.EndTable();
        }

        SharedToolSettings.Tip.Foreground = foreground.ToColor();
        SharedToolSettings.Tip.Background = background.ToColor();
        SharedToolSettings.Tip.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(mirror);
        SharedToolSettings.Tip.Glyph = glyph;

        ImGuiSC.EndGroupPanel();
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditingSurface.FontSize, Color.Green);

        if (!_isDrawing)
        {
            // Cancelled but left mouse finally released, exit cancelled
            if (_isCancelled && ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
                _isCancelled = false;

            // Cancelled
            if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && (ImGuiP.IsMouseClicked(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape)))
            {
                OnDeselected(document);
                _isCancelled = true;
                document.VisualLayerToolLower.Surface.Clear();
            }

            if (_isCancelled)
                return;

            if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
            {
                if (!_isFirstPointSelected)
                {
                    _isFirstPointSelected = true;

                    _firstPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;
                }

                _secondPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;

                document.VisualLayerToolLower.Surface.Clear();
                document.VisualLayerToolLower.Surface.DrawLine(_firstPoint,
                                         _secondPoint,
                                         SharedToolSettings.Tip.Glyph,
                                         SharedToolSettings.Tip.Foreground,
                                         SharedToolSettings.Tip.Background,
                                         SharedToolSettings.Tip.Mirror);
            }
            else if (ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (_firstPoint != Point.None)
                {
                    document.EditingSurface.Surface.DrawLine(_firstPoint + document.EditingSurface.Surface.ViewPosition,
                                             _secondPoint + document.EditingSurface.Surface.ViewPosition,
                                             SharedToolSettings.Tip.Glyph,
                                             SharedToolSettings.Tip.Foreground,
                                             SharedToolSettings.Tip.Background,
                                             SharedToolSettings.Tip.Mirror);


                    document.VisualLayerToolLower.Surface.Clear();
                }

                OnDeselected(document);
            }
        }
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document)
    {
        _isCancelled = false;
        _isDrawing = false;
        _firstPoint = Point.None;
        _isFirstPointSelected = false;
    }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}
