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
                                                              BoxBorderStyle = ImGuiTypes.ConnectedLineStyleType.AllConnectedLineStyles[0].ConnectedLineStyle };

    private ImGuiList<ImGuiTypes.ConnectedLineStyleType> _lineTypes = new(ImGuiTypes.ConnectedLineStyleType.AllConnectedLineStyles);

    public string Title => "\uefa4 Box";

    public string Description =>
        """
        Draws a box.

        The border and fill of the box can be customized.

        Depress the left mouse button to start drawing. Hold down the button and drag the mouse to draw the box. Let go of the button to finish drawing.

        To cancel drawing, depress the right mouse button or press the ESC key.
        """;

    public Box()
    {
        _lineTypes.SelectedItemIndex = 0;
    }

    public void BuildSettingsPanel(Document document)
    {
        ImGuiSC.BeginGroupPanel("Settings");

        ScreenSurface surface = document.EditingSurface;

        //SettingsTable.BeginTable("toolsettings");

        ImGui.Checkbox("Has Border", ref _shapeSettings.HasBorder);
        //SettingsTable.DrawCheckbox("Has Border", "##hasborder", ref _shapeSettings.HasBorder);
        if (_shapeSettings.HasBorder)
        {
            // Data for border settings
            ColoredGlyphReference borderGlyph = _shapeSettings.BorderGlyph ??= new ColoredGlyph();

            ImGuiSC.BeginGroupPanel("Border");
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

            SettingsTable.DrawCommonSettings(true, true, true, true, true,
                ref borderGlyph,
                surface.Surface.DefaultForeground.ToVector4(),
                surface.Surface.DefaultBackground.ToVector4(),
                document.EditingSurfaceFont, ImGuiCore.Renderer
            );

            SettingsTable.EndTable();
            if (ImGui.CollapsingHeader("Ignore Options##border"))
            {
                SettingsTable.BeginTable("bordersettings_ignore");
                SettingsTable.DrawCheckbox("Ignore Foreground", "##ignore_border_foreground", ref _shapeSettings.IgnoreBorderForeground);
                SettingsTable.DrawCheckbox("Ignore Background", "##ignore_border_background", ref _shapeSettings.IgnoreBorderBackground);
                SettingsTable.DrawCheckbox("Ignore Mirror", "##ignore_border_mirror", ref _shapeSettings.IgnoreBorderMirror);
                SettingsTable.DrawCheckbox("Ignore Glyph", "##ignore_border_glyph", ref _shapeSettings.IgnoreBorderGlyph);
                SettingsTable.EndTable();
            }
            // Store the altered settings
            _shapeSettings.BorderGlyph = borderGlyph.ToColoredGlyph();

            ImGuiSC.EndGroupPanel();
        }

        ImGui.Checkbox("Has Fill", ref _shapeSettings.HasFill);
        //SettingsTable.DrawCheckbox("Has Border", "##hasborder", ref _shapeSettings.HasBorder);
        if (_shapeSettings.HasFill)
        {
            _shapeSettings.FillGlyph ??= new ColoredGlyph();

            // Data for border settings
            ColoredGlyphReference fillGlyph = _shapeSettings.FillGlyph;

            ImGuiSC.BeginGroupPanel("Border");
            SettingsTable.BeginTable("bordersettings");

            SettingsTable.DrawCommonSettings(true, true, true, true, true,
                ref fillGlyph,
                surface.Surface.DefaultForeground.ToVector4(),
                surface.Surface.DefaultBackground.ToVector4(),
                document.EditingSurfaceFont, ImGuiCore.Renderer
            );

            SettingsTable.EndTable();

            if (ImGui.CollapsingHeader("Ignore Options##fill"))
            {
                SettingsTable.BeginTable("fillsettings_ignore");
                SettingsTable.DrawCheckbox("Ignore Foreground", "##ignore_Fill_foreground", ref _shapeSettings.IgnoreFillForeground);
                SettingsTable.DrawCheckbox("Ignore Background", "##ignore_Fill_background", ref _shapeSettings.IgnoreFillBackground);
                SettingsTable.DrawCheckbox("Ignore Mirror", "##ignore_Fill_mirror", ref _shapeSettings.IgnoreFillMirror);
                SettingsTable.DrawCheckbox("Ignore Glyph", "##ignore_Fill_glyph", ref _shapeSettings.IgnoreFillGlyph);
                SettingsTable.EndTable();
            }

            // Store the altered settings
            _shapeSettings.FillGlyph = fillGlyph.ToColoredGlyph();

            ImGuiSC.EndGroupPanel();
        }

        ImGuiSC.EndGroupPanel();
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {

    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}
