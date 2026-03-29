using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Types;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.Editor.Windows;

public static class GlyphEditorWindow
{
    private class Instance : ImGuiObjectBase
    {
        public ColoredGlyphReference Glyph;
        public string? Name;

        private readonly Vector4 _defaultForeground;
        private readonly Vector4 _defaultBackground;
        private readonly IFont _font;

        private Action<ColoredGlyphReference, string?> _onAccepted;
        private Action? _onCancelled;
        private bool _firstShow = true;

        public Instance(ColoredGlyph glyph, Color defaultForeground, Color defaultBackground, IFont font, Action<ColoredGlyphReference, string?> onAccepted, Action? onCancelled, string? name = null)
        {
            _defaultForeground = defaultForeground.ToVector4();
            _defaultBackground = defaultBackground.ToVector4();
            Glyph = glyph;
            _font = font;
            _onAccepted = onAccepted;
            _onCancelled = onCancelled;
            Name = name;
        }

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (_firstShow)
            {
                ImGui.OpenPopup("Glyph Editor"u8);
                _firstShow = false;
            }

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Core.Settings.WindowGlyphEditor * ImGui.GetFontSize(), -1));

            if (ImGui.BeginPopupModal("Glyph Editor"u8, ImGuiWindowFlags.NoResize))
            {
                SettingsTable.BeginTable("editglyph_table", column1Flags: ImGuiTableColumnFlags.WidthFixed);

                if (Name != null)
                {
                    SettingsTable.DrawString("Name", ref Name, 50);
                }

                SettingsTable.DrawCommonSettings(true, true, true, true, true,
                                                 ref Glyph, _defaultForeground, _defaultBackground, _font, renderer);

                SettingsTable.EndTable();

                if (ImGuiSC.WindowDrawButtons(out bool dialogResult, acceptButtonText: "Accept"))
                {
                    if (dialogResult)
                        _onAccepted(Glyph, Name);
                    else
                        _onCancelled?.Invoke();

                    ImGui.CloseCurrentPopup();
                    renderer.UIObjects.Remove(this);
                }
                ImGui.EndPopup();
            }
        }
    }

    public static void Show(ImGuiRenderer renderer, ColoredGlyph glyph, Color defaultForeground, Color defaultBackground, IFont font, Action<ColoredGlyphReference, string?> onAccepted, Action? onCancelled, string? name = null)
    {
        Instance instance = new(glyph, defaultForeground, defaultBackground, font, onAccepted, onCancelled, name);
        renderer.UIObjects.Add(instance);
    }
}
