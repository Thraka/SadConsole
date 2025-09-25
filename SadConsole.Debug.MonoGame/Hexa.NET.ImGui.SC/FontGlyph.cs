using System;
using System.Numerics;
using SadConsole;
using SadConsole.Host;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace Hexa.NET.ImGui.SC;

public static partial class ImGuiSC
{
    public static class FontGlyph
    {
        public static bool DrawWithPopup(ImGuiRenderer renderer, string id, string popupId, IFont font, Vector4 foreground, Vector4 background, ref int selectedGlyph, bool showNumber)
        {
            ImTextureID fontTexture = renderer.BindTexture(((GameTexture)font.Image).Texture);
            Rectangle rect = font.GetGlyphSourceRectangle(selectedGlyph);
            Point fontTextureSize = new Point(font.Image.Width, font.Image.Height);

            Vector2 renderAreaSize = font.GetFontSize(IFont.Sizes.Two).ToVector2();
            if (showNumber)
            {
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - ImGui.GetStyle().FramePadding.X * 2 - renderAreaSize.X);
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
                ImGui.OpenPopup("glyph_select");
            }

            ImGui.PopStyleVar();
            return GlyphPickerPopup.Show(renderer, "glyph_select", font, fontTexture, fontTextureSize, ref selectedGlyph);
        }

        public static void Draw(ImGuiRenderer renderer, string id, IFont font, ColoredGlyph glyph) =>
            Draw(renderer, id, font, glyph.Foreground.ToVector4(), glyph.Background.ToVector4(), glyph.Glyph);

        public static void Draw(ImGuiRenderer renderer, string id, IFont font, Vector4 foreground, Vector4 background, int glyph)
        {
            ImTextureID fontTexture = renderer.BindTexture(((GameTexture)font.Image).Texture);
            Rectangle rect = font.SolidGlyphRectangle;

            Point fontTextureSize = new(font.Image.Width, font.Image.Height);

            Vector2 renderAreaSize = font.GetFontSize(IFont.Sizes.Two).ToVector2();

            // TODO: Apply mirror to UV
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.One);
            var pos = ImGui.GetCursorPos();
            ImGui.Image(fontTexture,
                renderAreaSize,
                rect.Position.ToUV(fontTextureSize), (rect.Position + rect.Size).ToUV(fontTextureSize),
                background);

            ImGui.SetCursorPos(pos);

            rect = font.GetGlyphSourceRectangle(glyph);
            ImGui.Image(fontTexture,
                renderAreaSize,
                rect.Position.ToUV(fontTextureSize), (rect.Position + rect.Size).ToUV(fontTextureSize),
                foreground);
            ImGui.PopStyleVar();
        }
    }

}
