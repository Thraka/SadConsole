using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace Hexa.NET.ImGui;

public static partial class SettingsTable
{
    public static void BeginTable(string id)
    {
        if (ImGui.BeginTable(id, 2, ImGuiTableFlags.None, System.Numerics.Vector2.Zero, ImGui.GetContentRegionAvail().X))
        {
            ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("two", ImGuiTableColumnFlags.WidthStretch);
        }
    }

    public static void EndTable()
    {
        ImGui.EndTable();
    }

    public static void DrawCommonSettings(string id, bool showForeground, bool showBackground, bool showMirror, bool showGlyph, bool enableSwapForeBackRightClick,
                                          ref SadConsole.Debug.ColoredGlyphReference glyphRef, Vector4? foregroundResetColor, Vector4? backgroundResetColor,
                                          IFont font, ImGuiRenderer renderer)
    {
        DrawCommonSettings(id, showForeground, showBackground, showMirror, showGlyph, enableSwapForeBackRightClick,
                           ref glyphRef.Foreground, foregroundResetColor,
                           ref glyphRef.Background, backgroundResetColor,
                           ref glyphRef.Mirror, ref glyphRef.Glyph, font, renderer);
    }

    public static void DrawCommonSettings(string id, bool showForeground, bool showBackground, bool showMirror, bool showGlyph, bool enableSwapForeBackRightClick,
                                          ref Vector4 foreground, Vector4? foregroundResetColor,
                                          ref Vector4 background, Vector4? backgroundResetColor,
                                          ref Mirror mirror,
                                          ref int glyph, IFont font, ImGuiRenderer renderer)
    {
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
    }

    public static void DrawCheckbox(string label, string id, ref bool isChecked)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        ImGui.Checkbox(id, ref isChecked);
    }

    public static bool DrawInt(string label, string id, ref int intValue, int minValue = 0, int maxValue = -1)
    {
        bool returnValue;
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);
        ImGui.SetNextItemWidth(-1);
        returnValue = ImGui.InputInt(id, ref intValue);

        if (intValue < minValue)
            intValue = minValue;

        if (maxValue > -1 && intValue > maxValue)
            intValue = maxValue;

        return returnValue;
    }

    public static void DrawText(string label, string text)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);
        ImGui.SetNextItemWidth(-1);
        ImGui.Text(text);
    }

    public static bool DrawColor(string label, string id, ref Vector4 color, Vector4? resetColor, out bool colorRightClicked)
    {
        bool returnValue = false;
        Vector4 colorCopy = color;

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);

        ImGui.TableSetColumnIndex(1);
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.ColorEdit4(id, ref colorCopy, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs))
        {
            color = colorCopy;
            returnValue = true;
        }

        colorRightClicked = ImGui.IsItemClicked(ImGuiMouseButton.Right);

        if (resetColor.HasValue)
        {
            ImGui.SameLine();
            if (ImGui.Button($"Reset{id}"))
            {
                color = resetColor.Value;
                returnValue = true;
            }
        }

        return returnValue;
    }

    public static bool DrawMirror(string label, string id, ref Mirror mirror)
    {
        bool returnValue = false;
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        int itemIndex = Enums<Mirror>.GetIndexFromValue(mirror);

        if (ImGui.Combo(id, ref itemIndex, Enums<Mirror>.Names, Enums<Mirror>.Count))
        {
            mirror = Enums<Mirror>.GetValueFromIndex(itemIndex);
            returnValue = true;
        }

        return returnValue;
    }

    public static bool DrawFontGlyph(string label, string id, ref int glyph, Vector4 glyphForeground, Vector4 glyphBackground, IFont font, ImGuiRenderer renderer)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        return ImGui2.FontGlyph.DrawWithPopup(renderer, id, "glyph_select", font, glyphForeground, glyphBackground, ref glyph, true);
    }
}
