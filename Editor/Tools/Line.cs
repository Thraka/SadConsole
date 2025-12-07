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

    private bool _isDrawing = false;
    private bool _isFirstPointSelected = false;
    private Point _firstPoint;
    private Point _secondPoint;
    private bool _isCancelled;

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
            if (SharedToolSettings.ImGuiDrawObjects(document, out var obj))
            {
                _lineSettings.CopyAppearanceFrom(obj.Visual);
            }
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

                // Draw preview line based on current mode
                document.VisualLayerToolMiddle.Surface.DrawLine(_firstPoint,
                                         _secondPoint,
                                         _lineSettings.Glyph,
                                         _lineSettings.Foreground,
                                         _lineSettings.Background,
                                         _lineSettings.Mirror);
            }
            else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right))
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
            else if (ImGuiP.IsMouseReleased(ImGuiMouseButton.Left))
            {
                if (_firstPoint != Point.None)
                {
                    Point topLeft = _firstPoint + document.EditingSurface.Surface.ViewPosition;
                    Point bottomRight = _secondPoint + document.EditingSurface.Surface.ViewPosition;

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

                    document.VisualLayerToolMiddle.Surface.Clear();
                }

                ClearState();
            }
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
        _isDrawing = false;
        _firstPoint = Point.None;
        _isFirstPointSelected = false;
    }

    public override string ToString() =>
        Title;
}
