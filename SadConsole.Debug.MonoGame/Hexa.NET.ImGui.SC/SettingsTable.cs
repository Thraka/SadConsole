using System.Numerics;
using SadConsole;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace Hexa.NET.ImGui.SC;

public static partial class SettingsTable
{
    public static bool BeginTable(string id, ImGuiTableFlags tableFlags = ImGuiTableFlags.None,
                                             ImGuiTableColumnFlags column1Flags = ImGuiTableColumnFlags.WidthStretch,
                                             ImGuiTableColumnFlags column2Flags = ImGuiTableColumnFlags.WidthStretch)
    {
        if (ImGui.BeginTable(id, 2, ImGuiTableFlags.None, System.Numerics.Vector2.Zero, ImGui.GetContentRegionAvail().X))
        {
            ImGui.TableSetupColumn("one"u8, column1Flags);
            ImGui.TableSetupColumn("two"u8, column2Flags);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Call if <see cref="BeginTable"/> returns true."/>
    /// </summary>
    public static void EndTable()
    {
        ImGui.EndTable();
    }

    public static void DrawCommonSettings(bool showForeground, bool showBackground, bool showMirror, bool showGlyph, bool enableSwapForeBackRightClick,
                                          ref SadConsole.ImGuiTypes.ColoredGlyphReference glyphRef, Vector4? foregroundResetColor, Vector4? backgroundResetColor,
                                          IFont font, ImGuiRenderer renderer)
    {
        DrawCommonSettings(showForeground, showBackground, showMirror, showGlyph, enableSwapForeBackRightClick,
                           ref glyphRef.Foreground, foregroundResetColor,
                           ref glyphRef.Background, backgroundResetColor,
                           ref glyphRef.Mirror, ref glyphRef.Glyph, font, renderer);
    }

    public static void DrawCommonSettings(bool showForeground, bool showBackground, bool showMirror, bool showGlyph, bool enableSwapForeBackRightClick,
                                          ref Vector4 foreground, Vector4? foregroundResetColor,
                                          ref Vector4 background, Vector4? backgroundResetColor,
                                          ref SadConsole.ImGuiTypes.Mirror mirror,
                                          ref int glyph, IFont font, ImGuiRenderer renderer)
    {
        if (showForeground)
        {
            DrawColor("Foreground", "##fore", ref foreground, foregroundResetColor, true, out bool colorRightClicked);
            if (colorRightClicked && enableSwapForeBackRightClick)
                (background, foreground) = (foreground, background);
        }

        if (showBackground)
        {
            DrawColor("Background", "##back", ref background, backgroundResetColor, true, out bool colorRightClicked);
            if (colorRightClicked && enableSwapForeBackRightClick)
                (background, foreground) = (foreground, background);
        }

        if (showMirror)
            DrawMirror("Mirror", "##mirror", ref mirror);

        if (showGlyph)
            DrawFontGlyph("Glyph", "##glyph", ref glyph, foreground, background, font, renderer);
    }

    public static bool DrawCheckbox(string label, string id, ref bool isChecked)
    {
        bool returnValue;
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        returnValue = ImGui.Checkbox(id, ref isChecked);

        return returnValue;
    }

    public static bool DrawInt(string label, string id, ref int intValue, int minValue = 0, int maxValue = -1, int width = -1)
    {
        bool returnValue;

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);
        ImGui.SetNextItemWidth(width);
        returnValue = ImGui.InputInt(id, ref intValue);

        if (intValue < minValue)
            intValue = minValue;

        if (maxValue > -1 && intValue > maxValue)
            intValue = maxValue;

        return returnValue;
    }

    public static void DrawText(string label, string text, bool alignTextToFramePadding = true)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        if (alignTextToFramePadding)
            ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);
        ImGui.SetNextItemWidth(-1);
        ImGui.Text(text);
    }

    public static bool DrawString(string label, ref string text, ulong maxLength)
    {
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);
        ImGui.SetNextItemWidth(-1);
        bool edited = ImGui.InputText($"##drawstring_{label}", ref text, maxLength);
        return edited;
    }

    public static bool DrawColor(string label, string id, ref Vector4 color, Vector4? resetColor,
                                 bool showPalette,
                                 out bool colorRightClicked,
                                 ImGuiColorEditFlags flags = ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs)
    {
        bool returnValue = false;
        Vector4 colorCopy = color;

        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);

        ImGui.TableSetColumnIndex(1);
        //ImGui.SetNextItemWidth(-ImGui.GetContentRegionAvail().X);
        if (ImGui.ColorEdit4(id, ref colorCopy, flags))
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

        if (showPalette)
        {
            ImGui.SameLine();
            if (ImGui.Button($"Palette{id}"))
                ImGui.OpenPopup($"palettepopup_{id}");

            Color palColor = colorCopy.ToColor();
            if (Windows.PalettePopup.Show($"palettepopup_{id}", ref palColor))
                color = palColor.ToVector4();
        }

        return returnValue;
    }

    public static bool DrawMirror(string label, string id, ref SadConsole.ImGuiTypes.Mirror mirror, int width = -1)
    {
        bool returnValue = false;
        ImGui.TableNextRow();
        ImGui.TableSetColumnIndex(0);
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);
        ImGui.TableSetColumnIndex(1);

        int itemIndex = ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.GetIndexFromValue(mirror);

        ImGui.SetNextItemWidth(width);
        if (ImGui.Combo(id, ref itemIndex, ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.Names, ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.Count))
        {
            mirror = ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.GetValueFromIndex(itemIndex);
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

        return ImGuiSC.FontGlyph.DrawWithPopup(renderer, id, "glyph_select", font, glyphForeground, glyphBackground, ref glyph, true);
    }
}
