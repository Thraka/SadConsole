using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiTypes;

namespace SadConsole.Editor.Tools;

internal class Box : ITool
{
    private ImGuiTypes.ShapeSettings _shapeSettings = new() { HasBorder = true,
                                                              UseBoxBorderStyle = true,
                                                              BoxBorderStyle = ImGuiTypes.ConnectedLineStyleType.AllConnectedLineStyles[1].ConnectedLineStyle,
                                                              BorderGlyph = new ColoredGlyph(Color.White, Color.Black, 176),
                                                              HasFill = false,
                                                              FillGlyph = new ColoredGlyph()};

    private ImGuiTypes.ShapeSettings _shapeSettingsDraw = new()
    {
        HasBorder = true,
        UseBoxBorderStyle = true,
        BoxBorderStyle = ImGuiTypes.ConnectedLineStyleType.AllConnectedLineStyles[1].ConnectedLineStyle,
        BorderGlyph = new ColoredGlyph(Color.White, Color.Black, 176),
        HasFill = false,
        FillGlyph = new ColoredGlyph()
    };

    private ImGuiTypes.ShapeSettings _shapeSettingsErase = new()
    {
        HasBorder = true,
        UseBoxBorderStyle = true,
        BoxBorderStyle = ICellSurface.ConnectedLineThin,
        BorderGlyph = new ColoredGlyph(Color.White, Color.Black, 176),
        HasFill = true,
        FillGlyph = new ColoredGlyph(Color.White, Color.Gray.SetAlpha(100))
    };

    private ImGuiTypes.ShapeSettings _shapeSettingsOther = new()
    {
        HasBorder = true,
        UseBoxBorderStyle = true,
        BoxBorderStyle = ImGuiTypes.ConnectedLineStyleType.AllConnectedLineStyles[1].ConnectedLineStyle,
        BorderGlyph = new ColoredGlyph(Color.White, Color.Black, 176),
        HasFill = false,
        FillGlyph = new ColoredGlyph()
    };


    private ImGuiList<ImGuiTypes.ConnectedLineStyleType> _lineTypes = new(ImGuiTypes.ConnectedLineStyleType.AllConnectedLineStyles);

    private bool _isDrawing = false;
    private bool _isFirstPointSelected = false;
    private Point _firstPoint;
    private Point _secondPoint;
    private bool _isCancelled;

    public string Title => "\uefa4 Box";
    public ToolMode.Modes CurrentMode;

    public string Description =>
        """
        Draws a box.

        The border and fill of the box can be customized.

        Hold down the left mouse button to start drawing. Hold down the button and drag the mouse to draw the box. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public Box()
    {
        _lineTypes.SelectedItemIndex = 1;
    }

    public void BuildSettingsPanel(Document document)
    {
        ScreenSurface surface = document.EditingSurface;

        ImGui.AlignTextToFramePadding();
        ImGui.Text("Mode"u8);
        ImGui.SameLine();

        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        if (ImGui.Combo("##toolmode", ref document.ToolModes.SelectedItemIndex, document.ToolModes.Names, document.ToolModes.Count))
            ConfigureToolMode(document);

        // Drawing mode
        if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Draw)
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

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Checkbox("Use Line Style", ref _shapeSettings.UseBoxBorderStyle);
                    ImGui.TableSetColumnIndex(1);

                    ImGui.BeginDisabled(!_shapeSettings.UseBoxBorderStyle);
                    ImGui.Combo("##border_line_style", ref _lineTypes.SelectedItemIndex, _lineTypes.Names, _lineTypes.Count);
                    ImGui.EndDisabled();

                    if (_shapeSettings.UseBoxBorderStyle)
                        _shapeSettings.BoxBorderStyle = _lineTypes.SelectedItem!.ConnectedLineStyle;
                    else
                        _shapeSettings.BoxBorderStyle = null;

                    if (_shapeSettings.UseBoxBorderStyle)
                    {
                        SettingsTable.DrawColor("Foreground", "##borderFore", ref borderGlyph.Foreground, surface.Surface.DefaultForeground.ToVector4(), true, out bool rightClick);
                        SettingsTable.DrawColor("Background", "##borderBack", ref borderGlyph.Background, surface.Surface.DefaultBackground.ToVector4(), true, out rightClick);
                    }
                    else
                    {
                        SettingsTable.DrawCommonSettings(true, true, true, true, true,
                            ref borderGlyph,
                            surface.Surface.DefaultForeground.ToVector4(),
                            surface.Surface.DefaultBackground.ToVector4(),
                            document.EditingSurfaceFont, ImGuiCore.Renderer
                        );
                    }

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
        // Objects mode
        else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Objects)
        {
            ImGui.Checkbox("Has Border", ref _shapeSettings.HasBorder);
            ImGui.Checkbox("Has Fill", ref _shapeSettings.HasFill);

            if (SharedToolSettings.ImGuiDrawObjects(document, out var obj))
            {
                if (_shapeSettings.HasBorder)
                {
                    _shapeSettings.UseBoxBorderStyle = false;
                    _shapeSettings.BoxBorderStyle = null;
                    _shapeSettings.BorderGlyph = obj.Visual;
                }

                if (_shapeSettings.HasFill)
                    _shapeSettings.FillGlyph = obj.Visual;
            }
        }
    }

    private void ConfigureToolMode(Document document)
    {
        // Moving from an old mode, capture shape settings
        switch (CurrentMode)
        {
            case ToolMode.Modes.Draw:
                _shapeSettingsDraw = _shapeSettings;
                break;
            case ToolMode.Modes.Objects:
            case ToolMode.Modes.Zones:
                _shapeSettingsOther = _shapeSettings;
                break;
            default:
                break;
        }

        // Move to new mode
        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        // Clear the box layer
        ClearState();
        document.VisualLayerToolMiddle.Surface.DefaultBackground = Color.Transparent;
        document.VisualLayerToolMiddle.Clear();

        if (CurrentMode == ToolMode.Modes.Empty)
        {
            _shapeSettings = _shapeSettingsErase;

            document.VisualLayerToolLower.Surface.DefaultBackground = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            document.VisualLayerToolLower.Clear();

            var clearCell = new ColoredGlyph(document.EditingSurface.Surface.DefaultForeground, document.EditingSurface.Surface.DefaultBackground, 0);

            for (int index = 0; index < document.VisualLayerToolLower.Surface.Surface.Count; index++)
            {
                ColoredGlyphBase renderCell = document.EditingSurface.Surface[(Point.FromIndex(index, document.VisualLayerToolLower.Surface.Surface.Width) + document.EditingSurface.Surface.ViewPosition).ToIndex(document.EditingSurface.Surface.Width)];

                if (renderCell.Foreground == clearCell.Foreground &&
                    renderCell.Background == clearCell.Background &&
                    renderCell.Glyph == clearCell.Glyph)

                    document.VisualLayerToolLower.Surface.Surface[index].Background = Core.Settings.EmptyCellColor;
            }
        }
        else if (CurrentMode == ToolMode.Modes.Draw)
        {
            _shapeSettings = _shapeSettingsDraw;

            // Reset empty tool mode
            document.VisualLayerToolLower.Surface.DefaultBackground = Color.Transparent;
            document.VisualLayerToolLower.Clear();
        }
        else
        {
            _shapeSettings = _shapeSettingsOther;

            // Reset empty tool mode
            document.VisualLayerToolLower.Surface.DefaultBackground = Color.Transparent;
            document.VisualLayerToolLower.Clear();
        }
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.ViewPosition, document.EditorFontSize, Color.Green);

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
                document.VisualLayerToolMiddle.Surface.DrawBox(new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                                                            new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                                                                            _shapeSettings.ToShapeParameters());
            }

            // Commit
            else if (ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
            {
                Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
                Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

                // TODO: Can you even get Point.None here?
                if (_firstPoint != Point.None)
                {
                    // Draw box or object mode
                    if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Draw || document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Objects)
                    {
                        document.EditingSurface.Surface.DrawBox(new Rectangle(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                                                                new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y))),
                                                                _shapeSettings.ToShapeParameters());
                    }

                    // Empty cell mode
                    else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Empty)
                    {
                        // Erase the editing surface
                        document.EditingSurface.Surface.Clear(
                            new Rectangle(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                          new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y))));

                        // Fill the empty tool layer
                        document.VisualLayerToolLower.Surface.Fill(
                            new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                          new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                            background: Core.Settings.EmptyCellColor);
                    }
                }

                document.VisualLayerToolMiddle.Surface.Clear();
                ClearState();
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
