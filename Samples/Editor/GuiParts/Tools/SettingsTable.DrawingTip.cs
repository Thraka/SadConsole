using ImGuiNET;
using System.Numerics;
using SadConsole.ImGuiSystem;
using SadConsole.Editor.Model;
using SadConsole.Editor.Tools;
using static Microsoft.Xna.Framework.Graphics.SpriteFont;
using static System.Net.Mime.MediaTypeNames;
using SadConsole.Editor.Model.SadConsoleTypes;
using SadConsole.Editor.Windows;

namespace SadConsole.Editor.GuiParts.Tools;

internal static partial class SettingsTable
{
    public static void DrawCheckbox(string label, string id, ref bool isChecked)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        ImGui.Checkbox(id, ref isChecked);
    }

    public static void DrawInt(string label, string id, ref int intValue, int minValue = 0, int maxValue = -1)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        ImGui.InputInt(id, ref intValue);

        if (intValue < minValue)
            intValue = minValue;

        if (maxValue > -1 && intValue > maxValue)
            intValue = maxValue;
    }

    public static void DrawColor(string label, string id, ref Vector4 color, Vector4? resetColor, out bool colorRightClicked)
    {
        Vector4 colorCopy = color;

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);

        ImGui.TableSetColumnIndex(1);
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.ColorEdit4(id, ref colorCopy, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs))
            color = colorCopy;
        
        colorRightClicked = ImGui.IsItemClicked(ImGuiMouseButton.Right);

        ImGui.SameLine();
        if (ImGui.Button($"Palette{id}"))
        {
            ImGuiCore.State.OpenPopup($"palettepopup##{id}");
        }

        Color col = color.ToColor();
        if (PalettePopup.Show($"palettepopup##{id}", ref col))
            color = col.ToVector4();

        if (resetColor.HasValue)
        {
            ImGui.SameLine();
            if (ImGui.Button($"Reset{id}"))
                color = resetColor.Value;
        }
    }

    public static void DrawFontGlyph(string label, string id, ref int glyph, Vector4 glyphForeground, Vector4 glyphBackground, IFont font, ImGuiRenderer renderer)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        FontGlyph.DrawWithPopup(renderer, id, "glyph_select", font, glyphForeground, glyphBackground, ref glyph, true);
    }

    public static void DrawMirror(string label, string id, ref Mirror mirror)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        int itemIndex = Model.SadConsoleTypes.Mirror.GetIndexFromValue(mirror);

        if (ImGui.Combo(id, ref itemIndex, Model.SadConsoleTypes.Mirror.Names, Model.SadConsoleTypes.Mirror.Names.Length))
        {
            mirror = Model.SadConsoleTypes.Mirror.GetValueFromIndex(itemIndex);
        }
    }

    public static void DrawCommonSettings(string id, bool showForeground, bool showBackground, bool showMirror, bool showGlyph, bool enableSwapForeBackRightClick,
                                          ref Vector4 foreground, Vector4? foregroundResetColor,
                                          ref Vector4 background, Vector4? backgroundResetColor,
                                          ref Mirror mirror, 
                                          ref int glyph, IFont font, ImGuiRenderer renderer)
    {
        BeginTable(id);

        if (showForeground)
        {
            DrawColor("Foreground:", "##fore", ref foreground, foregroundResetColor, out bool colorRightClicked);
            if (colorRightClicked && enableSwapForeBackRightClick)
                (background, foreground) = (foreground, background);
        }

        if (showBackground)
        {
            DrawColor("Background:", "##back", ref background, backgroundResetColor, out bool colorRightClicked);
            if (colorRightClicked && enableSwapForeBackRightClick)
                (background, foreground) = (foreground, background);
        }

        if (showMirror)
            DrawMirror("Mirror:", "##mirror", ref mirror);

        if (showGlyph)
            DrawFontGlyph("Glyph:", "##glyph", ref glyph, foreground, background, font, renderer);

        EndTable();
    }
}
