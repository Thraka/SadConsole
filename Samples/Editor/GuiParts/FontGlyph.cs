using System.Numerics;
using SadConsole.Editor;
using SadConsole.ImGuiSystem;
using SadConsole.Host;
using static Microsoft.Xna.Framework.Graphics.SpriteFont;
using SadConsole.Editor.Windows;

namespace ImGuiNET;

public static class FontGlyph
{
    public static void DrawWithPopup(ImGuiRenderer renderer, string id, string popupId, IFont font, Vector4 foreground, Vector4 background, ref int selectedGlyph, bool showNumber)
    {
        nint fontTexture = renderer.BindTexture(((GameTexture)font.Image).Texture);
        Rectangle rect = font.GetGlyphSourceRectangle(selectedGlyph);
        Point fontTextureSize = new Point(font.Image.Width, font.Image.Height);

        Vector2 renderAreaSize = font.GetFontSize(IFont.Sizes.Two).ToVector2();

        if (showNumber)
        {
            if (ImGui.InputInt(id, ref selectedGlyph, 1))
                selectedGlyph = Math.Clamp(selectedGlyph, 0, font.TotalGlyphs - 1);

            ImGui.SameLine();
        }

        // TODO: Apply mirror to UV
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.One);
        if (ImGui.ImageButton($"{id}##tool_tip_settings_button", fontTexture,
                              renderAreaSize,
                              rect.Position.ToUV(fontTextureSize), (rect.Position + rect.Size).ToUV(fontTextureSize),
                              background, foreground))
        {
            ImGuiCore.State.OpenPopup("glyph_select");
        }

        ImGui.PopStyleVar();
        GlyphPickerPopup.Show(renderer, "glyph_select", font, fontTexture, fontTextureSize, ref selectedGlyph);
        ImGuiCore.State.CheckSetPopupOpen("glyph_select");
    }

    public static void Draw(ImGuiRenderer renderer, string id, IFont font, Vector4 foreground, Vector4 background, int glyph)
    {
        nint fontTexture = renderer.BindTexture(((GameTexture)font.Image).Texture);
        Rectangle rect = font.GetGlyphSourceRectangle(glyph);
        Point fontTextureSize = new Point(font.Image.Width, font.Image.Height);

        Vector2 renderAreaSize = font.GetFontSize(IFont.Sizes.Two).ToVector2();

        // TODO: Apply mirror to UV
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.One);
        ImGui.BeginDisabled();
        ImGui.ImageButton($"{id}##tool_tip_settings_button", fontTexture,
                              renderAreaSize,
                              rect.Position.ToUV(fontTextureSize), (rect.Position + rect.Size).ToUV(fontTextureSize),
                              background, foreground);
        ImGui.EndDisabled();
        ImGui.PopStyleVar();
    }
}
