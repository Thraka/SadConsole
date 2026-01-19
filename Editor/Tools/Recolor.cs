using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using Hexa.NET.ImGui.SC.Windows;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Recolor : ITool
{
    private bool _isMatchForeground = false;
    private bool _isMatchBackground = false;
    private bool _isMatchGlyph = false;
    private bool _isMatchMirror = false;

    private Vector4 _matchForeground = Color.White.ToVector4();
    private Vector4 _matchBackground = Color.Black.ToVector4();
    private int _matchGlyph = 0;
    private Mirror _matchMirror = Mirror.None;

    private bool _isApplyForeground = false;
    private bool _isApplyBackground = false;
    private bool _isApplyGlyph = false;
    private bool _isApplyMirror = false;

    private Vector4 _applyForeground = Color.White.ToVector4();
    private Vector4 _applyBackground = Color.Black.ToVector4();
    private int _applyGlyph = 0;
    private Mirror _applyMirror = Mirror.None;

    public string Title => "\ue22b Recolor";

    public string Description => """
        Recolor the glyphs on the surface.

        Set the match conditions, then use the left-mouse button to apply the new colors based on the match conditions.

        The right-mouse button changes the current condition parameters to match the foreground, background, and glyph that is under the cursor.
        """;

    public void BuildSettingsPanel(Document document)
    {
        IScreenSurface surface = document.EditingSurface;

        ImGuiSC.BeginGroupPanel("Match");

        //
        // MATCH SETTINGS
        //
        if (SettingsTable.BeginTable("matchtable"))
        {

            // Foreground
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Foreground", ref _isMatchForeground);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isMatchForeground);

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##foreground_edit", ref _matchForeground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                (_matchForeground, _matchBackground) = (_matchBackground, _matchForeground);

            ImGui.SameLine();
            if (ImGui.Button($"Palette##foreground_edit"))
                ImGui.OpenPopup($"palettepopup##foreground_edit");

            Color col = _matchForeground.ToColor();
            if (PalettePopup.Show($"palettepopup##foreground_edit", ref col))
                _matchForeground = col.ToVector4();

            ImGui.EndDisabled();

            // Background
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Background", ref _isMatchBackground);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isMatchBackground);

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##background_edit", ref _matchBackground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                (_matchForeground, _matchBackground) = (_matchBackground, _matchForeground);

            ImGui.SameLine();
            if (ImGui.Button($"Palette##background_edit"))
                ImGui.OpenPopup($"palettepopup##background_edit");

            col = _matchBackground.ToColor();
            if (PalettePopup.Show($"palettepopup##background_edit", ref col))
                _matchBackground = col.ToVector4();

            ImGui.EndDisabled();

            // Mirror
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Mirror", ref _isMatchMirror);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isMatchMirror);
            int itemIndex =
                ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.GetIndexFromValue(ImGuiTypes.MirrorConverter.FromSadConsoleMirror(_matchMirror));

            if (ImGui.Combo("##mirror_edit", ref itemIndex, ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.Names,
                    ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.Count))
                _matchMirror =
                    ImGuiTypes.MirrorConverter.ToSadConsoleMirror(ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.GetValueFromIndex(itemIndex));
            ImGui.EndDisabled();

            // Glyph
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Glyph", ref _isMatchGlyph);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isMatchGlyph);
            ImGuiSC.FontGlyph.DrawWithPopup(ImGuiCore.Renderer, "##glyph_edit", "match_font", surface.Font, _matchForeground, _matchBackground,
                ref _matchGlyph, true);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                (_applyGlyph, _matchGlyph) = (_matchGlyph, _applyGlyph);
            ImGui.EndDisabled();

            SettingsTable.EndTable();
        }

        ImGuiSC.EndGroupPanel();


        ImGuiSC.BeginGroupPanel("Apply");

        //
        // APPLY SETTINGS
        //

        if (SettingsTable.BeginTable("applytable"))
        {

            // Foreground
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Foreground", ref _isApplyForeground);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isApplyForeground);

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##foreground_edit", ref _applyForeground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                (_applyForeground, _applyBackground) = (_applyBackground, _applyForeground);

            ImGui.SameLine();
            if (ImGui.Button($"Palette##foreground_edit"))
                ImGui.OpenPopup($"palettepopup##foreground_edit");

            Color col = _applyForeground.ToColor();
            if (PalettePopup.Show($"palettepopup##foreground_edit", ref col))
                _applyForeground = col.ToVector4();

            ImGui.EndDisabled();

            // Background
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Background", ref _isApplyBackground);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isApplyBackground);

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##background_edit", ref _applyBackground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                (_applyForeground, _applyBackground) = (_applyBackground, _applyForeground);

            ImGui.SameLine();
            if (ImGui.Button($"Palette##background_edit"))
                ImGui.OpenPopup($"palettepopup##background_edit");

            col = _applyBackground.ToColor();
            if (PalettePopup.Show($"palettepopup##background_edit", ref col))
                _applyBackground = col.ToVector4();

            ImGui.EndDisabled();

            // Mirror
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Mirror", ref _isApplyMirror);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isApplyMirror);
            int itemIndex = ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.GetIndexFromValue(ImGuiTypes.MirrorConverter.FromSadConsoleMirror(_applyMirror));

            if (ImGui.Combo("##mirror_edit", ref itemIndex, ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.Names,
                    ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.Count))
                _applyMirror =
                    ImGuiTypes.MirrorConverter.ToSadConsoleMirror(ImGuiListEnum<SadConsole.ImGuiTypes.Mirror>.GetValueFromIndex(itemIndex));
            ImGui.EndDisabled();

            // Glyph
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Glyph", ref _isApplyGlyph);
            ImGui.TableSetColumnIndex(1);

            ImGui.BeginDisabled(!_isApplyGlyph);

            ImGuiSC.FontGlyph.DrawWithPopup(ImGuiCore.Renderer, "##glyph_edit", "apply_font", surface.Font, _applyForeground, _applyBackground,
                ref _applyGlyph, true);

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                (_applyGlyph, _matchGlyph) = (_matchGlyph, _applyGlyph);
            ImGui.EndDisabled();

            SettingsTable.EndTable();
        }

        ImGuiSC.EndGroupPanel();

    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.EditingSurface.Surface.ViewPosition, document.EditorFontSize, Color.Green);

        if (!isActive) return;

        if (ImGuiP.IsMouseDown(ImGuiMouseButton.Left))
        {
            ColoredGlyphBase targetCell = document.EditingSurface.Surface[hoveredCellPosition];

            bool matched = false;

            matched = _isMatchForeground || _isMatchBackground || _isMatchMirror || _isMatchGlyph;
            matched &= !_isMatchForeground || (_isMatchForeground && targetCell.Foreground == _matchForeground.ToColor());
            matched &= !_isMatchBackground || (_isMatchBackground && targetCell.Background == _matchBackground.ToColor());
            matched &= !_isMatchMirror || (_isMatchMirror && targetCell.Mirror == _matchMirror);
            matched &= !_isMatchGlyph || (_isMatchGlyph && targetCell.Glyph == _matchGlyph);

            // If found, apply
            if (matched)
            {
                if (_isApplyForeground)
                    targetCell.Foreground = _applyForeground.ToColor();
                if (_isApplyBackground)
                    targetCell.Background = _applyBackground.ToColor();
                if (_isApplyMirror)
                    targetCell.Mirror = _applyMirror;
                if (_isApplyGlyph)
                    targetCell.Glyph = _applyGlyph;

                document.EditingSurface.IsDirty = true;
            }
        }
        else if (ImGuiP.IsMouseDown(ImGuiMouseButton.Right))
        {
            ColoredGlyphBase tempCell = document.EditingSurface.Surface[hoveredCellPosition];

            if (ImGuiP.IsKeyDown(ImGuiKey.ModShift))
            {
                _applyForeground = tempCell.Foreground.ToVector4();
                _applyBackground = tempCell.Background.ToVector4();
                _applyMirror = tempCell.Mirror;
                _applyGlyph = tempCell.Glyph;
            }
            else
            {
                _matchForeground = tempCell.Foreground.ToVector4();
                _matchBackground = tempCell.Background.ToVector4();
                _matchMirror = tempCell.Mirror;
                _matchGlyph = tempCell.Glyph;
            }
        }
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}
