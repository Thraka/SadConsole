using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiTypes;

namespace SadConsole.Editor.Tools;

internal class Line : ITool
{
    private ColoredGlyph _lineSettings = new ColoredGlyph(Color.White, Color.Black, 176);
    private ColoredGlyph _lineSettingsDraw = new ColoredGlyph(Color.White, Color.Black, 176);
    private ColoredGlyph _lineSettingsErase = new ColoredGlyph(Core.Settings.EmptyCellColor, Core.Settings.EmptyCellColor.GetDarker().SetAlpha(100), '*');
    private ColoredGlyph _lineSettingsOther = new ColoredGlyph(Color.White, Color.Black, 176);

    private ZoneSimplified? _currentZone;

    private bool _isFirstPointSelected = false;
    private Point _firstPoint;
    private Point _secondPoint;
    private bool _isCancelled;
    private bool _isRightClickDrag;

    public string Title => "\ue216 Line";
    public ToolMode.Modes CurrentMode;

    public string Description => """
        Draws a line.

        The line can be set to use a connected glyph.

        Depress the left mouse button to start drawing. Hold down the button and drag the mouse to draw the line. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

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
            ImGuiSC.BeginGroupPanel("Settings");

            Vector4 foreground = _lineSettings.Foreground.ToVector4();
            Vector4 background = _lineSettings.Background.ToVector4();
            ImGuiTypes.Mirror mirror = ImGuiTypes.MirrorConverter.FromSadConsoleMirror(_lineSettings.Mirror);
            int glyph = _lineSettings.Glyph;

            if (SettingsTable.BeginTable("linesettings", column1Flags: ImGuiTableColumnFlags.WidthFixed))
            {
                SettingsTable.DrawCommonSettings(true, true, true, true, true,
                    ref foreground, surface.Surface.DefaultForeground.ToVector4(),
                    ref background, surface.Surface.DefaultBackground.ToVector4(),
                    ref mirror,
                    ref glyph, surface.Font, ImGuiCore.Renderer);
                SettingsTable.EndTable();
            }

            _lineSettings.Foreground = foreground.ToColor();
            _lineSettings.Background = background.ToColor();
            _lineSettings.Mirror = ImGuiTypes.MirrorConverter.ToSadConsoleMirror(mirror);
            _lineSettings.Glyph = glyph;

            ImGuiSC.EndGroupPanel();
        }
        // Objects mode
        else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Objects)
        {
            if (SimpleObjectHelpers.ImGuiDrawObjectsList(document, out var obj))
            {
                _lineSettings.CopyAppearanceFrom(obj.Visual);
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
        // Moving from an old mode, capture line settings
        switch (CurrentMode)
        {
            case ToolMode.Modes.Draw:
                _lineSettingsDraw = _lineSettings;
                break;
            case ToolMode.Modes.Objects:
            case ToolMode.Modes.Zones:
                _lineSettingsOther = _lineSettings;
                break;
            default:
                break;
        }

        // Move to new mode
        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        // Clear the line layer
        ClearState();
        document.VisualLayerToolMiddle.Surface.DefaultBackground = Color.Transparent;
        document.VisualLayerToolMiddle.Clear();

        if (CurrentMode == ToolMode.Modes.Empty)
        {
            _lineSettings = _lineSettingsErase;

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
            _lineSettings = _lineSettingsOther;

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
            _lineSettings = _lineSettingsDraw;

            // Reset empty tool mode
            document.VisualLayerToolLower.Surface.DefaultBackground = Color.Transparent;
            document.VisualLayerToolLower.Clear();
        }
        else
        {
            _lineSettings = _lineSettingsOther;

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
        if (CurrentMode == ToolMode.Modes.Objects)
        {
            // Objects mode requires a valid object (handled by SharedToolSettings)
        }

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
                // Draw preview line with clear appearance
                document.VisualLayerToolMiddle.Surface.DrawLine(_firstPoint,
                                         _secondPoint,
                                         '*',
                                         Color.White,
                                         Color.Red.SetAlpha(100));
            }
            // For zone add mode, use the zone's appearance
            else if (CurrentMode == ToolMode.Modes.Zones)
            {
                document.VisualLayerToolMiddle.Surface.DrawLine(_firstPoint,
                                         _secondPoint,
                                         _currentZone!.Appearance.Glyph,
                                         _currentZone.Appearance.Foreground,
                                         _currentZone.Appearance.Background);
            }
            else
            {
                // Draw preview line based on current mode
                document.VisualLayerToolMiddle.Surface.DrawLine(_firstPoint,
                                         _secondPoint,
                                         _lineSettings.Glyph,
                                         _lineSettings.Foreground,
                                         _lineSettings.Background,
                                         _lineSettings.Mirror);
            }
        }
        else if (!_isFirstPointSelected && ImGuiP.IsMouseDown(ImGuiMouseButton.Right) && CurrentMode != ToolMode.Modes.Zones)
        {
            ColoredGlyphBase sourceCell = document.EditingSurface.Surface[hoveredCellPosition];

            if (ImGuiP.IsKeyDown(ImGuiKey.ModShift))
            {
                _lineSettings.Foreground = sourceCell.Foreground;
                _lineSettings.Background = sourceCell.Background;
            }
            else if (ImGuiP.IsKeyDown(ImGuiKey.ModCtrl))
            {
                _lineSettings.Glyph = sourceCell.Glyph;
            }
            else
                document.EditingSurface.Surface[hoveredCellPosition].CopyAppearanceTo(_lineSettings);
        }

        // Commit
        else if (isDrawingButtonReleased && _isFirstPointSelected)
        {
            Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
            Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

            if (_firstPoint != Point.None)
            {
                // Draw line or object mode
                if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Draw || document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Objects)
                {
                    document.EditingSurface.Surface.DrawLine(topLeft,
                                             bottomRight,
                                             _lineSettings.Glyph,
                                             _lineSettings.Foreground,
                                             _lineSettings.Background,
                                             _lineSettings.Mirror);
                }

                // Empty cell mode
                else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Empty)
                {
                    // Get all points along the line
                    foreach (Point point in Lines.GetBresenhamLine(topLeft, bottomRight))
                    {
                        if (document.EditingSurface.Surface.IsValidCell(point))
                        {
                            document.EditingSurface.Surface.Clear(point.X, point.Y, 1);

                            Point visualPoint = point - document.EditingSurface.Surface.ViewPosition;
                            if (document.VisualLayerToolLower.Surface.IsValidCell(visualPoint))
                            {
                                document.VisualLayerToolLower.Surface.Surface[visualPoint].Background = Core.Settings.EmptyCellColor;
                            }
                        }
                    }

                    document.VisualLayerToolLower.Surface.IsDirty = true;
                }

                // Zones mode
                else if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Zones && _currentZone != null)
                {
                    // Right-click drag clears the zone area
                    if (_isRightClickDrag)
                    {
                        if (_currentZone.ZoneArea != null)
                        {
                            // Remove all positions along the line from the zone
                            foreach (Point point in Lines.GetBresenhamLine(topLeft, bottomRight))
                            {
                                _currentZone.ZoneArea.Remove(point);
                            }

                            // Clear the visual layer for the line
                            foreach (Point point in Lines.GetBresenhamLine(_firstPoint, _secondPoint))
                            {
                                if (point.X >= 0 && point.X < document.VisualLayerToolLower.Surface.Surface.Width &&
                                    point.Y >= 0 && point.Y < document.VisualLayerToolLower.Surface.Surface.Height)
                                {
                                    document.VisualLayerToolLower.Surface.Clear(point.X, point.Y);
                                }
                            }
                        }
                    }
                    // Left-click drag adds to the zone area
                    else
                    {
                        _currentZone.ZoneArea ??= new();

                        // Add all positions along the line to the zone
                        foreach (Point point in Lines.GetBresenhamLine(topLeft, bottomRight))
                        {
                            _currentZone.ZoneArea.Add(point);
                        }

                        // Update the visual layer for the line
                        foreach (Point point in Lines.GetBresenhamLine(_firstPoint, _secondPoint))
                        {
                            if (point.X >= 0 && point.X < document.VisualLayerToolLower.Surface.Surface.Width &&
                                point.Y >= 0 && point.Y < document.VisualLayerToolLower.Surface.Surface.Height)
                            {
                                var cell = document.VisualLayerToolLower.Surface.Surface[point];
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

    public void OnSelected(Document document)
    {
        // Copy from shared tip to draw settings only
        SharedToolSettings.Tip.CopyAppearanceTo(_lineSettingsDraw);
        
        ConfigureToolMode(document);
    }

    public void OnDeselected(Document document)
    {
        // Copy draw settings back to shared tip
        _lineSettingsDraw.CopyAppearanceTo(SharedToolSettings.Tip);
        
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
