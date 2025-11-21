using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiTypes;

namespace SadConsole.Editor.Windows;

public class GlyphEditor : ImGuiWindowBase
{
    public ColoredGlyphReference Glyph;
    public string? Name;

    private readonly Vector4 _defaultForeground;
    private readonly Vector4 _defaultBackground;
    private readonly IFont _font;

    public GlyphEditor(ColoredGlyph glyph, Color defaultForeground, Color defaultBackground, IFont font, string? name = null)
    {
        Title = "Glyph Editor";
        _defaultForeground = defaultForeground.ToVector4();
        _defaultBackground = defaultBackground.ToVector4();
        Glyph = glyph;
        _font = font;
        Name = name;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Core.Settings.WindowGlyphEditor * ImGui.GetFontSize(), -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                SettingsTable.BeginTable("editglyph_table", column1Flags: ImGuiTableColumnFlags.WidthFixed);

                if (Name != null)
                {
                    SettingsTable.DrawString("Name", ref Name, 50);
                }

                SettingsTable.DrawCommonSettings(true,true, true, true,true,
                                                 ref Glyph, _defaultForeground, _defaultBackground, _font, renderer);

                SettingsTable.EndTable();

                if (DrawButtons(out DialogResult, false, acceptButtonText: "Accept"))
                    Close();

                ImGui.EndPopup();
            }
        }
    }
}
