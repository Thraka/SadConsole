using System.Numerics;
using ImGuiNET;
using SadConsole.Components;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Box : ITool, IOverlay
{
    private bool _isFirstPointSelected = false;
    private Rectangle _boxArea;
    private Point _firstPoint;
    private Point _secondPoint;
    private Overlay _toolOverlay = new();
    private bool _isCancelled;

    private ShapeSettings.Settings _shapeSettings;

    public string Name => "Box";

    public string Description => """
        Draws a box.

        The border and fill of the box can be customized.

        Depress the left mouse button to start drawing. Hold down the button and drag the mouse to draw the box. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public Overlay Overlay => _toolOverlay;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGuiWidgets.BeginGroupPanel("Settings");

        IScreenSurface surface = ImGuiCore.State.GetOpenDocument().Surface;

        //GuiParts.Tools.SettingsTable.BeginTable("toolsettings");

        ImGui.Checkbox("Has Border", ref _shapeSettings.HasBorder);
        //GuiParts.Tools.SettingsTable.DrawCheckbox("Has Border", "##hasborder", ref _shapeSettings.HasBorder);
        if (_shapeSettings.HasBorder)
        {

            _shapeSettings.BorderGlyph ??= new ColoredGlyph();

            // Data for border settings
            Vector4 foreground = _shapeSettings.BorderGlyph.Foreground.ToVector4();
            Vector4 background = _shapeSettings.BorderGlyph.Background.ToVector4();
            Mirror mirror = _shapeSettings.BorderGlyph.Mirror;
            int glyph = _shapeSettings.BorderGlyph.Glyph;

            ImGuiWidgets.BeginGroupPanel("Border");
            GuiParts.Tools.SettingsTable.BeginTable("bordersettings");

            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Use Line Style", ref _shapeSettings.UseBoxBorderStyle);
            ImGui.TableSetColumnIndex(1);

            int itemIndex = Model.SadConsoleTypes.ConnectedGlyphs.GetIndexFromValue(_shapeSettings.BoxBorderStyle ?? ICellSurface.ConnectedLineEmpty);

            ImGui.Combo("##border_line_style", ref itemIndex, Model.SadConsoleTypes.ConnectedGlyphs.Names, Model.SadConsoleTypes.ConnectedGlyphs.Names.Length);
            
            if (_shapeSettings.UseBoxBorderStyle)
                _shapeSettings.BoxBorderStyle = Model.SadConsoleTypes.ConnectedGlyphs.GetValueFromIndex(itemIndex);
            else
                _shapeSettings.BoxBorderStyle = null;


            GuiParts.Tools.SettingsTable.DrawColor("Foreground:", "##fore", ref foreground, surface.Surface.DefaultForeground.ToVector4(), out bool colorRightClicked);
            if (colorRightClicked)
                (background, foreground) = (foreground, background);

            GuiParts.Tools.SettingsTable.DrawColor("Background:", "##back", ref background, surface.Surface.DefaultBackground.ToVector4(), out colorRightClicked);
            if (colorRightClicked)
                (background, foreground) = (foreground, background);

            GuiParts.Tools.SettingsTable.DrawMirror("Mirror:", "##mirror", ref mirror);

            GuiParts.Tools.SettingsTable.DrawFontGlyph("Glyph:", "##glyph", ref glyph, foreground, background, surface.Font, renderer);
            GuiParts.Tools.SettingsTable.EndTable();
            if (ImGui.CollapsingHeader("Ignore Options##border"))
            {
                GuiParts.Tools.SettingsTable.BeginTable("bordersettings_ignore");
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Foreground", "##ignore_border_foreground", ref _shapeSettings.IgnoreBorderForeground);
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Background", "##ignore_border_background", ref _shapeSettings.IgnoreBorderBackground);
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Mirror", "##ignore_border_mirror", ref _shapeSettings.IgnoreBorderMirror);
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Glyph", "##ignore_border_glyph", ref _shapeSettings.IgnoreBorderGlyph);
                GuiParts.Tools.SettingsTable.EndTable();
            }
            // Store the altered settings
            _shapeSettings.BorderGlyph.Foreground = foreground.ToColor();
            _shapeSettings.BorderGlyph.Background = background.ToColor();
            _shapeSettings.BorderGlyph.Mirror = mirror;
            _shapeSettings.BorderGlyph.Glyph = glyph;

            ImGuiWidgets.EndGroupPanel();
        }

        ImGui.Checkbox("Has Fill", ref _shapeSettings.HasFill);
        //GuiParts.Tools.SettingsTable.DrawCheckbox("Has Border", "##hasborder", ref _shapeSettings.HasBorder);
        if (_shapeSettings.HasFill)
        {

            _shapeSettings.FillGlyph ??= new ColoredGlyph();

            // Data for border settings
            Vector4 foreground = _shapeSettings.FillGlyph.Foreground.ToVector4();
            Vector4 background = _shapeSettings.FillGlyph.Background.ToVector4();
            Mirror mirror = _shapeSettings.FillGlyph.Mirror;
            int glyph = _shapeSettings.FillGlyph.Glyph;

            ImGuiWidgets.BeginGroupPanel("Border");
            GuiParts.Tools.SettingsTable.BeginTable("bordersettings");

            GuiParts.Tools.SettingsTable.DrawColor("Foreground:", "##fore", ref foreground, surface.Surface.DefaultForeground.ToVector4(), out bool colorRightClicked);
            if (colorRightClicked)
                (background, foreground) = (foreground, background);

            GuiParts.Tools.SettingsTable.DrawColor("Background:", "##back", ref background, surface.Surface.DefaultBackground.ToVector4(), out colorRightClicked);
            if (colorRightClicked)
                (background, foreground) = (foreground, background);

            GuiParts.Tools.SettingsTable.DrawMirror("Mirror:", "##mirror", ref mirror);

            GuiParts.Tools.SettingsTable.DrawFontGlyph("Glyph:", "##glyph", ref glyph, foreground, background, surface.Font, renderer);
            GuiParts.Tools.SettingsTable.EndTable();
            if (ImGui.CollapsingHeader("Ignore Options##fill"))
            {
                GuiParts.Tools.SettingsTable.BeginTable("fillsettings_ignore");
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Foreground", "##ignore_Fill_foreground", ref _shapeSettings.IgnoreFillForeground);
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Background", "##ignore_Fill_background", ref _shapeSettings.IgnoreFillBackground);
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Mirror", "##ignore_Fill_mirror", ref _shapeSettings.IgnoreFillMirror);
                GuiParts.Tools.SettingsTable.DrawCheckbox("Ignore Glyph", "##ignore_Fill_glyph", ref _shapeSettings.IgnoreFillGlyph);
                GuiParts.Tools.SettingsTable.EndTable();
            }

            // Store the altered settings
            _shapeSettings.FillGlyph.Foreground = foreground.ToColor();
            _shapeSettings.FillGlyph.Background = background.ToColor();
            _shapeSettings.FillGlyph.Mirror = mirror;
            _shapeSettings.FillGlyph.Glyph = glyph;

            ImGuiWidgets.EndGroupPanel();
        }

        ImGuiWidgets.EndGroupPanel();
    }

    public void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        GuiParts.Tools.ToolHelpers.HighlightCell(hoveredCellPosition, surface.Surface.ViewPosition, surface.FontSize, Color.Green);

        // No settings to draw, exit
        if (!_shapeSettings.HasFill && !_shapeSettings.HasBorder)
            return;

        // Cancelled but left mouse finally released, exit cancelled
        if (_isCancelled && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            _isCancelled = false;

        // Cancelled
        if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && (ImGui.IsMouseClicked(ImGuiMouseButton.Right) || ImGui.IsKeyReleased(ImGuiKey.Escape)))
        {
            OnDeselected();
            _isCancelled = true;
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

            _boxArea = new(new Point(Math.Min(_firstPoint.X, _secondPoint.X), Math.Min(_firstPoint.Y, _secondPoint.Y)),
                            new Point(Math.Max(_firstPoint.X, _secondPoint.X), Math.Max(_firstPoint.Y, _secondPoint.Y)));

            Overlay.Surface.Clear();
            Overlay.Surface.DrawBox(_boxArea, _shapeSettings.ToShapeParameters());
        }
        else if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            if (_boxArea != Rectangle.Empty)
            {
                surface.Surface.DrawBox(_boxArea.Translate(surface.Surface.ViewPosition), _shapeSettings.ToShapeParameters());
                Overlay.Surface.Clear();
            }

            OnDeselected();
        }
    }

    public void OnSelected()
    {

    }

    public void OnDeselected()
    {
        Overlay.Surface.Clear();
        _isCancelled = false;
        _boxArea = Rectangle.Empty;
        _isFirstPointSelected = false;
    }

    public void DocumentViewChanged() { }

    public void DrawOverDocument() { }
}
