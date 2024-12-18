﻿using System;
using System.Numerics;
using SadConsole;
using SadConsole.Host;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace Hexa.NET.ImGui;

public static partial class ImGui2
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

        public static void Draw(ImGuiRenderer renderer, string id, IFont font, Vector4 foreground, Vector4 background, int glyph)
        {
            ImTextureID fontTexture = renderer.BindTexture(((GameTexture)font.Image).Texture);
            Rectangle rect = font.GetGlyphSourceRectangle(glyph);
            Point fontTextureSize = new(font.Image.Width, font.Image.Height);

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

}
