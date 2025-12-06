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
    public ToolMode.Modes CurrentMode;

    public string Description => """
        Draws a line.

        The line can be set to use a connected glyph.

        Depress the left mouse button to start drawing. Hold down the button and drag the mouse to draw the line. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public void BuildSettingsPanel(Document document)
    {
        ImGui.SeparatorText(Title);

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
    }

    private void ConfigureToolMode(Document document)
    {
        CurrentMode = document.ToolModes.SelectedItem!.Mode;

        // Clear the line layer
        ClearState();
        document.VisualLayerToolMiddle.Surface.DefaultBackground = Color.Transparent;
        document.VisualLayerToolMiddle.Clear();

        if (CurrentMode == ToolMode.Modes.Empty)
        {
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
        else
        {
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
                if (document.ToolModes.SelectedItem!.Mode == ToolMode.Modes.Empty)
                {
                    // Preview for empty mode - use * glyph with empty cell color
                    document.VisualLayerToolMiddle.Surface.DrawLine(_firstPoint,
                                             _secondPoint,
                                             '*',
                                             Core.Settings.EmptyCellColor.GetDarker(),
                                             Core.Settings.EmptyCellColor,
                                             Mirror.None);
                }
                else
                {
                    // Preview for draw/objects mode - use current tip settings
                    document.VisualLayerToolMiddle.Surface.DrawLine(_firstPoint,
                                             _secondPoint,
                                             SharedToolSettings.Tip.Glyph,
                                             SharedToolSettings.Tip.Foreground,
                                             SharedToolSettings.Tip.Background,
                                             SharedToolSettings.Tip.Mirror);
                }
            }
            else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right))
            {
                ColoredGlyphBase sourceCell = document.EditingSurface.Surface[hoveredCellPosition];

                if (ImGuiP.IsKeyDown(ImGuiKey.ModShift))
                {
                    SharedToolSettings.Tip.Foreground = sourceCell.Foreground;
                    SharedToolSettings.Tip.Background = sourceCell.Background;
                }
                else if (ImGuiP.IsKeyDown(ImGuiKey.ModCtrl))
                {
                    SharedToolSettings.Tip.Glyph = sourceCell.Glyph;
                }
                else
                    document.EditingSurface.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
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
                                                 SharedToolSettings.Tip.Glyph,
                                                 SharedToolSettings.Tip.Foreground,
                                                 SharedToolSettings.Tip.Background,
                                                 SharedToolSettings.Tip.Mirror);
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
