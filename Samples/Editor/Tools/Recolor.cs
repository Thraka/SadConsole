using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.Editor.Windows;
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

    public string Name => "Recolor";

    public string Description => """
        Recolor the glyphs on the surface.
        
        Set the match conditions, then use the left-mouse button to apply the new colors based on the match conditions.
        
        The right-mouse button changes the current condition parameters to match the foreground, background, and glyph that is under the cursor.
        """;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        IScreenSurface surface = ((IDocumentSurface)ImGuiCore.State.GetOpenDocument()).Surface;

        ImGuiWidgets.BeginGroupPanel("Match");

        GuiParts.Tools.SettingsTable.BeginTable("matchtable");

        // Foreground
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Foreground", ref _isMatchForeground);
            ImGui.TableSetColumnIndex(1);

            if (!_isMatchForeground) ImGui.BeginDisabled();

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##foreground_edit", ref _matchForeground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            ImGuiCore.State.CheckSetPopupOpen($"##foregroundpicker");

            ImGui.SameLine();
            if (ImGui.Button($"Palette##foreground_edit"))
                ImGuiCore.State.OpenPopup($"palettepopup##foreground_edit");

            Color col = _matchForeground.ToColor();
            if (PalettePopup.Show($"palettepopup##foreground_edit", ref col))
                _matchForeground = col.ToVector4();

            if (!_isMatchForeground) ImGui.EndDisabled();
        }

        // Background
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Background", ref _isMatchBackground);
            ImGui.TableSetColumnIndex(1);

            if (!_isMatchBackground) ImGui.BeginDisabled();

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##background_edit", ref _matchBackground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            ImGuiCore.State.CheckSetPopupOpen($"##backgroundpicker");

            ImGui.SameLine();
            if (ImGui.Button($"Palette##background_edit"))
                ImGuiCore.State.OpenPopup($"palettepopup##background_edit");

            Color col = _matchBackground.ToColor();
            if (PalettePopup.Show($"palettepopup##background_edit", ref col))
                _matchBackground = col.ToVector4();

            if (!_isMatchBackground) ImGui.EndDisabled();
        }

        // Mirror
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Mirror", ref _isMatchMirror);
            ImGui.TableSetColumnIndex(1);

            if (!_isMatchMirror) ImGui.BeginDisabled();

            int itemIndex = Model.SadConsoleTypes.Mirror.GetIndexFromValue(_matchMirror);

            if (ImGui.Combo("##mirror_edit", ref itemIndex, Model.SadConsoleTypes.Mirror.Names, Model.SadConsoleTypes.Mirror.Names.Length))
            {
                _matchMirror = Model.SadConsoleTypes.Mirror.GetValueFromIndex(itemIndex);
            }

            if (!_isMatchMirror) ImGui.EndDisabled();
        }

        // Glyph
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Glyph", ref _isMatchGlyph);
            ImGui.TableSetColumnIndex(1);

            if (!_isMatchGlyph) ImGui.BeginDisabled();

            FontGlyph.DrawWithPopup(renderer, "##glyph_edit", "match_font", surface.Font, _matchForeground, _matchBackground, ref _matchGlyph,true);

            if (!_isMatchGlyph) ImGui.EndDisabled();
        }

        GuiParts.Tools.SettingsTable.EndTable();
        ImGuiWidgets.EndGroupPanel();


        ImGuiWidgets.BeginGroupPanel("Apply");

        GuiParts.Tools.SettingsTable.BeginTable("applytable");

        // Foreground
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Foreground", ref _isApplyForeground);
            ImGui.TableSetColumnIndex(1);

            if (!_isApplyForeground) ImGui.BeginDisabled();

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##foreground_edit", ref _applyForeground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            ImGuiCore.State.CheckSetPopupOpen($"##foregroundpicker");

            ImGui.SameLine();
            if (ImGui.Button($"Palette##foreground_edit"))
                ImGuiCore.State.OpenPopup($"palettepopup##foreground_edit");

            Color col = _applyForeground.ToColor();
            if (PalettePopup.Show($"palettepopup##foreground_edit", ref col))
                _applyForeground = col.ToVector4();

            if (!_isApplyForeground) ImGui.EndDisabled();
        }

        // Background
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Background", ref _isApplyBackground);
            ImGui.TableSetColumnIndex(1);

            if (!_isApplyBackground) ImGui.BeginDisabled();

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.ColorEdit4("##background_edit", ref _applyBackground, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);
            ImGuiCore.State.CheckSetPopupOpen($"##backgroundpicker");

            ImGui.SameLine();
            if (ImGui.Button($"Palette##background_edit"))
                ImGuiCore.State.OpenPopup($"palettepopup##background_edit");

            Color col = _applyBackground.ToColor();
            if (PalettePopup.Show($"palettepopup##background_edit", ref col))
                _applyBackground = col.ToVector4();

            if (!_isApplyBackground) ImGui.EndDisabled();
        }

        // Mirror
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Mirror", ref _isApplyMirror);
            ImGui.TableSetColumnIndex(1);

            if (!_isApplyMirror) ImGui.BeginDisabled();

            int itemIndex = Model.SadConsoleTypes.Mirror.GetIndexFromValue(_applyMirror);

            if (ImGui.Combo("##mirror_edit", ref itemIndex, Model.SadConsoleTypes.Mirror.Names, Model.SadConsoleTypes.Mirror.Names.Length))
            {
                _applyMirror = Model.SadConsoleTypes.Mirror.GetValueFromIndex(itemIndex);
            }

            if (!_isApplyMirror) ImGui.EndDisabled();
        }

        // Glyph
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Checkbox("Glyph", ref _isApplyGlyph);
            ImGui.TableSetColumnIndex(1);

            if (!_isApplyGlyph) ImGui.BeginDisabled();

            FontGlyph.DrawWithPopup(renderer, "##glyph_edit", "apply_font", surface.Font, _applyForeground, _applyBackground, ref _applyGlyph, true);

            if (!_isApplyGlyph) ImGui.EndDisabled();
        }

        GuiParts.Tools.SettingsTable.EndTable();
        ImGuiWidgets.EndGroupPanel();

    }

    public void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, surface.Surface.ViewPosition, surface.FontSize, Color.Green);

        if (!isActive) return;

        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            ColoredGlyphBase targetCell = surface.Surface[hoveredCellPosition];

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

                surface.IsDirty = true;
            }
        }
        else if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
        {
            ColoredGlyphBase tempCell = surface.Surface[hoveredCellPosition];

            _matchForeground = tempCell.Foreground.ToVector4();
            _matchBackground = tempCell.Background.ToVector4();
            _matchMirror = tempCell.Mirror;
            _matchGlyph = tempCell.Glyph;
        }
    }

    public void OnSelected() { }

    public void OnDeselected() { }

    public void DocumentViewChanged() { }

    public void DrawOverDocument() { }
}
