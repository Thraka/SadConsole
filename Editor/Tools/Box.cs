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

    private ZoneSimplified? _currentZone;
    private SimpleObjectDefinition? _currentObject;

    private ImGuiList<ImGuiTypes.ConnectedLineStyleType> _lineTypes = new(ImGuiTypes.ConnectedLineStyleType.AllConnectedLineStyles);

    private bool _isFirstPointSelected = false;
    private Point _firstPoint;
    private Point _secondPoint;
    private bool _isCancelled;
    private bool _isRightClickDrag;

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
        IScreenSurface surface = document.EditingSurface;

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

            if (SimpleObjectHelpers.ImGuiDrawObjectsList(document, out _currentObject))
            {
                if (_shapeSettings.HasBorder)
                {
                    _shapeSettings.UseBoxBorderStyle = false;
                    _shapeSettings.BoxBorderStyle = null;
                    _shapeSettings.BorderGlyph = _currentObject.Visual;
                }

                if (_shapeSettings.HasFill)
                    _shapeSettings.FillGlyph = _currentObject.Visual;
            }
        }

        // Zones mode
        else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Zones)
        {
            ZoneHelpers.ImGuiDrawZones(document, out _currentZone, out bool zoneVisualChanged);

            if (zoneVisualChanged)
                ConfigureToolMode(document);
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
        else if (CurrentMode == ToolMode.Modes.Zones)
        {
            _shapeSettings = _shapeSettingsOther;

            // Set a darkened semi-transparent background for the zone layer
            document.VisualLayerToolLower.Surface.DefaultBackground = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            document.VisualLayerToolLower.Clear();

            // Get zones from document
            IDocumentZones docZones = (IDocumentZones)document;
            Point viewPosition = document.EditingSurface.Surface.ViewPosition;
            int viewWidth = document.VisualLayerToolLower.Surface.Surface.Width;
            int viewHeight = document.VisualLayerToolLower.Surface.Surface.Height;

            // Iterate through all zones and color their positions
            foreach (var zone in docZones.Zones.Objects)
            {
                if (zone.ZoneArea == null) continue;

                foreach (Point position in zone.ZoneArea)
                {
                    // Convert from surface coordinates to view coordinates
                    int viewX = position.X - viewPosition.X;
                    int viewY = position.Y - viewPosition.Y;

                    // Check if position is within the current viewport
                    if (viewX >= 0 && viewX < viewWidth && viewY >= 0 && viewY < viewHeight)
                    {
                        var cell = document.VisualLayerToolLower.Surface.Surface[viewX, viewY];
                        zone.Appearance.CopyAppearanceTo(cell);
                    }
                }
            }

            document.VisualLayerToolLower.Surface.IsDirty = true;
        }
        else if (CurrentMode == ToolMode.Modes.Draw)
        {
            _shapeSettings = _shapeSettingsDraw;

            // Reset empty tool mode
            document.VisualLayerToolLower.Surface.DefaultBackground = Color.Transparent;
            document.VisualLayerToolLower.Clear();
        }

        // Objects
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

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        // Cancelled but mouse button finally released, exit cancelled
        if (_isCancelled)
        {
            if (!ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && !ImGuiP.IsMouseDown(ImGuiMouseButton.Right))
                _isCancelled = false;

            return;
        }

        // Cancelled - only for non-zone modes
        if (CurrentMode != ToolMode.Modes.Zones)
        {
            if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left) && (ImGuiP.IsMouseClicked(ImGuiMouseButton.Right) || ImGuiP.IsKeyReleased(ImGuiKey.Escape)))
            {
                ClearState();
                _isCancelled = true;
                document.VisualLayerToolMiddle.Surface.Clear();
            }

            if (_isCancelled)
                return;
        }

        // For zones mode, handle escape to cancel
        if (CurrentMode == ToolMode.Modes.Zones && _isFirstPointSelected
            && (ImGuiP.IsKeyReleased(ImGuiKey.Escape) ||
                    (ImGuiP.IsMouseClicked(ImGuiMouseButton.Right) && !_isRightClickDrag) ||
                    (ImGuiP.IsMouseClicked(ImGuiMouseButton.Left) && _isRightClickDrag)
               )
           )
        {
            ClearState();
            _isCancelled = true;
            document.VisualLayerToolMiddle.Surface.Clear();
            return;
        }

        // Determine which mouse button is being used for drawing
        bool isLeftDown = ImGuiP.IsMouseDown(ImGuiMouseButton.Left);
        bool isRightDown = ImGuiP.IsMouseDown(ImGuiMouseButton.Right);
        bool isLeftReleased = ImGuiP.IsMouseReleased(ImGuiMouseButton.Left);
        bool isRightReleased = ImGuiP.IsMouseReleased(ImGuiMouseButton.Right);

        // For zones mode, allow right-click to start a clear operation
        bool isDrawingButton = isLeftDown || (CurrentMode == ToolMode.Modes.Zones && isRightDown && !_isFirstPointSelected);
        bool isDrawingButtonReleased = _isRightClickDrag ? isRightReleased : isLeftReleased;

        // Track if we started with right-click (for zones clear)
        if (CurrentMode == ToolMode.Modes.Zones && isRightDown && !_isFirstPointSelected && !isLeftDown)
            _isRightClickDrag = true;

        else if (!_isFirstPointSelected && isLeftDown)
            _isRightClickDrag = false;

        // Continue tracking the correct button during drag
        if (_isFirstPointSelected)
        {
            isDrawingButton = _isRightClickDrag ? isRightDown : isLeftDown;
        }

        // For zones mode, require a zone to be selected before drawing
        if (CurrentMode == ToolMode.Modes.Zones && _currentZone == null)
            return;

        // For objects mode, require an object to be selected before drawing
        if (CurrentMode == ToolMode.Modes.Objects && _currentObject == null)
            return;

        // Preview
        if (isDrawingButton && isActive)
        {
            if (!_isFirstPointSelected)
            {
                _isFirstPointSelected = true;
                _firstPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;
            }

            _secondPoint = hoveredCellPosition - document.EditingSurface.Surface.ViewPosition;

            document.VisualLayerToolMiddle.Surface.Clear();

            // For zone clear mode, use a different visual
            if (CurrentMode == ToolMode.Modes.Zones && _isRightClickDrag)
            {
                // Draw a simple filled rectangle to indicate clear area
                document.VisualLayerToolMiddle.Surface.Fill(
                    new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                    new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                    foreground: Color.White,
                    background: Color.Red.SetAlpha(100));
            }
            // For zone add mode, use the zone's appearance
            else if (CurrentMode == ToolMode.Modes.Zones)
            {
                // Fill the entire box area with the zone's appearance
                document.VisualLayerToolMiddle.Surface.Fill(
                    new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                    new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                    _currentZone!.Appearance.Foreground,
                    _currentZone.Appearance.Background,
                    _currentZone.Appearance.Glyph);
            }
            else
            {
                document.VisualLayerToolMiddle.Surface.DrawBox(new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                                                            new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y))),
                                                                            _shapeSettings.ToShapeParameters());
            }
        }

        // Commit
        else if (isDrawingButtonReleased && _isFirstPointSelected)
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

                // Zones mode
                else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Zones && _currentZone != null)
                {
                    Rectangle boxRect = new Rectangle(new Point(Math.Min(topLeft.X, bottomRight.X), Math.Min(topLeft.Y, bottomRight.Y)),
                                                        new Point(Math.Max(topLeft.X, bottomRight.X), Math.Max(topLeft.Y, bottomRight.Y)));

                    Rectangle viewRect = new Rectangle(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                                                        new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y)));

                    // Right-click drag clears the zone area
                    if (_isRightClickDrag)
                    {
                        if (_currentZone.ZoneArea != null)
                        {
                            // Remove all positions in the box from the zone
                            foreach (Point position in boxRect.Positions())
                            {
                                _currentZone.ZoneArea.Remove(position);
                            }

                            // Clear the visual layer for the box area
                            foreach (Point position in viewRect.Positions())
                            {
                                if (position.X >= 0 && position.X < document.VisualLayerToolLower.Surface.Surface.Width &&
                                    position.Y >= 0 && position.Y < document.VisualLayerToolLower.Surface.Surface.Height)
                                {
                                    document.VisualLayerToolLower.Surface.Clear(position.X, position.Y);
                                }
                            }
                        }
                    }
                    // Left-click drag adds to the zone area
                    else
                    {
                        _currentZone.ZoneArea ??= new();

                        // Add all positions in the box to the zone
                        foreach (Point position in boxRect.Positions())
                        {
                            _currentZone.ZoneArea.Add(position);
                        }

                        // Update the visual layer for the box area
                        foreach (Point position in viewRect.Positions())
                        {
                            if (position.X >= 0 && position.X < document.VisualLayerToolLower.Surface.Surface.Width &&
                                position.Y >= 0 && position.Y < document.VisualLayerToolLower.Surface.Surface.Height)
                            {
                                var cell = document.VisualLayerToolLower.Surface.Surface[position];
                                _currentZone.Appearance.CopyAppearanceTo(cell);
                            }
                        }
                    }

                    document.VisualLayerToolLower.Surface.IsDirty = true;
                }
            }

            document.VisualLayerToolMiddle.Surface.Clear();
            ClearState();
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
        _firstPoint = Point.None;
        _isFirstPointSelected = false;
        _isRightClickDrag = false;
    }

    public override string ToString() =>
        Title;
}
