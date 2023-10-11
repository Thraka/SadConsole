using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.Entities;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Debug.MonoGame.ImGuiTypes;

internal class ImGuiColoredGlyphEditor
{
    [System.Flags]
    public enum Modes
    {
        None = 0,
        Glyph = 1,
        Fore = 2,
        Back = 4
    }

    SadConsole.ColoredGlyphBase _glyph;
    ColoredGlyph _imGuiGlyph;
    Modes _mode;

    public Vector4 TextColor = Color.White.ToVector4();

    public bool BuildUI(string id, ImGuiRenderer renderer, SadConsole.ColoredGlyphBase coloredGlyph, IFont font, Modes mode)
    {
        bool changed = false;

        if (_glyph != coloredGlyph)
        {
            _glyph = coloredGlyph;
            _imGuiGlyph = _glyph;
            _mode = mode;
        }

        ImGui.PushID(id);
        ImGui.BeginGroup();
        {
            var fontTexture = renderer.BindTexture(((Host.GameTexture)font.Image).Texture);
            var rect = font.GetGlyphSourceRectangle(_imGuiGlyph.Glyph);
            var textureSize = new Point(font.Image.Width, font.Image.Height);

            ImGui.AlignTextToFramePadding();

            if ((mode & Modes.Glyph) == Modes.Glyph)
            {
                ImGui.TextColored(TextColor, $"Glyph: (#{_imGuiGlyph.Glyph})");
                ImGui.SameLine();
                if (ImGui.ImageButton(fontTexture, font.GetFontSize(IFont.Sizes.One).ToVector2(), rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize)))
                { }
                ImGui.SameLine();
            }

            if ((mode & Modes.Fore) == Modes.Fore)
            {
                ImGui.TextColored(TextColor, "Fore:");
                ImGui.SameLine();
                if (ImGui.ColorEdit4("##fore", ref _imGuiGlyph.Foreground, ImGuiColorEditFlags.NoInputs))
                {
                    _glyph.Foreground = _imGuiGlyph.Foreground.ToColor();
                    changed = true;
                }
                ImGui.SameLine();
            }

            if ((mode & Modes.Back) == Modes.Back)
            {
                ImGui.TextColored(TextColor, "Back:");
                ImGui.SameLine();
                if (ImGui.ColorEdit4("##back", ref _imGuiGlyph.Background, ImGuiColorEditFlags.NoInputs))
                {
                    _glyph.Background = _imGuiGlyph.Background.ToColor();
                    changed = true;
                }
            }
        }
        ImGui.EndGroup();
        ImGui.PopID();

        return changed;
    }
}
