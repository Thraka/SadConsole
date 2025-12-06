using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiTypes;

namespace SadConsole.Editor.Tools;

internal class Circle : ITool
{
    private ImGuiTypes.ShapeSettings _shapeSettings = new() { HasBorder = true,
                                                              UseBoxBorderStyle = false,
                                                              BorderGlyph = new ColoredGlyph(Color.White, Color.Black, 176),
                                                              HasFill = false,
                                                              FillGlyph = new ColoredGlyph()};

    private bool _isDrawing = false;
    private bool _isFirstPointSelected = false;
    private Point _firstPoint;
    private Point _secondPoint;
    private bool _isCancelled;

    public string Title => "\uf10c Circle";
    public ToolMode.Modes CurrentMode;

    public string Description =>
        """
        Draws a circle.

        The border and fill of the circle can be customized.

        Hold down the left mouse button to start drawing. Hold down the button and drag the mouse to draw the circle. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public Circle()
    {
    }

    public void BuildSettingsPanel(Document document)
    {
        ImGui.SeparatorText(Title);

        ScreenSurface surface = document.EditingSurface;

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Mode"u8);
        ImGui.SameLine();

        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        if (ImGui.Combo("##toolmode", ref document.ToolModes.SelectedItemIndex, document.ToolModes.Names, document.ToolModes.Count))
            ConfigureToolMode(document);

        // Drawing mode
        if (CurrentMode == ToolMode.Modes.Draw)
        {
            ImGui.Checkbox("Has Border", ref _shapeSettings.HasBorder);
            //SettingsTable.DrawCheckbox("Has Border", "##hasborder", ref _shapeSettings.HasBorder);
            if (_shapeSettings.HasBorder)
            {
                if (ImGui.CollapsingHeader("Border Options"))
                {
                    // Data for border settings
                    ColoredGlyphReference borderGlyph = _shapeSettings.BorderGlyph ??= new ColoredGlyph();

                    SettingsTable.BeginTable("bordersettings");

                    SettingsTable.DrawCommonSettings(true, true, true, true, true,
                        ref borderGlyph,
                        surface.Surface.DefaultForeground.ToVector4(),
                        surface.Surface.DefaultBackground.ToVector4(),
                        document.EditingSurfaceFont, ImGuiCore.Renderer
                    );

                    SettingsTable.DrawCheckbox("Ignore Foreground", "##ignore_border_foreground", ref _shapeSettings.IgnoreBorderForeground);
                    SettingsTable.DrawCheckbox("Ignore Background", "##ignore_border_background", ref _shapeSettings.IgnoreBorderBackground);
                    SettingsTable.DrawCheckbox("Ignore Mirror", "##ignore_border_mirror", ref _shapeSettings.IgnoreBorderMirror);
                    SettingsTable.DrawCheckbox("Ignore Glyph", "##ignore_border_glyph", ref _shapeSettings.IgnoreBorderGlyph);

                    SettingsTable.EndTable();
                    // Store the altered settings
                    _shapeSettings.BorderGlyph = borderGlyph.ToColoredGlyph();
                }
            }

            // Show a separator line if the previous section is shown
            if (_shapeSettings.HasBorder)
                ImGui.Separator();

            ImGui.Checkbox("Has Fill", ref _shapeSettings.HasFill);

            //SettingsTable.DrawCheckbox("Has Border", "##hasborder", ref _shapeSettings.HasBorder);

            if (_shapeSettings.HasFill)
            {
                if (ImGui.CollapsingHeader("Fill Options"))
                {
                    _shapeSettings.FillGlyph ??= new ColoredGlyph();

                    // Data for border settings
                    ColoredGlyphReference fillGlyph = _shapeSettings.FillGlyph;

                    SettingsTable.BeginTable("fillsettings");

                    SettingsTable.DrawCommonSettings(true, true, true, true, true,
                        ref fillGlyph,
                        surface.Surface.DefaultForeground.ToVector4(),
                        surface.Surface.DefaultBackground.ToVector4(),
                        document.EditingSurfaceFont, ImGuiCore.Renderer
                    );

                    SettingsTable.DrawCheckbox("Ignore Foreground", "##ignore_Fill_foreground", ref _shapeSettings.IgnoreFillForeground);
                    SettingsTable.DrawCheckbox("Ignore Background", "##ignore_Fill_background", ref _shapeSettings.IgnoreFillBackground);
                    SettingsTable.DrawCheckbox("Ignore Mirror", "##ignore_Fill_mirror", ref _shapeSettings.IgnoreFillMirror);
                    SettingsTable.DrawCheckbox("Ignore Glyph", "##ignore_Fill_glyph", ref _shapeSettings.IgnoreFillGlyph);

                    SettingsTable.EndTable();

                    // Store the altered settings
                    _shapeSettings.FillGlyph = fillGlyph.ToColoredGlyph();
                }
            }
        }
        else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Empty)
        {
            ImGui.Text("Erase mode is not supported.");
        }
    }

    private void ConfigureToolMode(Document document)
    {
        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        // Clear the circle layer
        ClearState();
        document.VisualLayerToolMiddle.Surface.DefaultBackground = Color.Transparent;
        document.VisualLayerToolMiddle.Clear();

        // Only Draw mode is supported - no special configuration needed
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

        if (CurrentMode == ToolMode.Modes.Draw)
        {
            if (!_isDrawing)
            {
                // Cancelled but left mouse finally released, exit cancelled
                if (_isCancelled && ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
                    _isCancelled = false;

                // Cancelled
                if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && (ImGuiP.IsMouseClicked(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape)))
                {
                    ClearState();
                    _isCancelled = true;
                    document.VisualLayerToolMiddle.Surface.Clear();
                }

                if (_isCancelled)
                    return;

                // Preview
                if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && isActive)
                {
                    if (!_isFirstPointSelected)
                    {
                        _isFirstPointSelected = true;

                        _firstPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;
                    }

                    _secondPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;

                    document.VisualLayerToolMiddle.Surface.Clear();
                    document.VisualLayerToolMiddle.Surface.DrawCircle(new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                                                                   new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                                                                     _shapeSettings.ToShapeParameters());
                }

                // Commit
                else if (ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    if (_firstPoint != Point.None)
                    {
                        Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
                        Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;
                        document.EditingSurface.Surface.DrawCircle(new Rectangle(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                                                                 new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y))),
                                                                   _shapeSettings.ToShapeParameters());

                        document.VisualLayerToolMiddle.Surface.Clear();
                    }

                    ClearState();
                }
            }
        }
    }

    public void OnSelected(Document document) =>
        ConfigureToolMode(document);

    public void OnDeselected(Document document)
    {
        ClearState();
        document.ResetVisualLayers();
    }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) =>
        ConfigureToolMode(document);

    public void DrawOverDocument(Document document) { }

    public void ClearState()
    {
        _isCancelled = false;
        _isDrawing = false;
        _firstPoint = Point.None;
        _isFirstPointSelected = false;
    }

    public override string ToString() =>
        Title;
}
