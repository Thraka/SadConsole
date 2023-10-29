using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.Ansi;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;
internal class Pencil : ITool
{
    public string Name => "Pencil";

    private ColoredGlyph _tip = CommonToolSettings.Tip;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGui.TextDisabled("(?)");
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 25.0f);
            ImGui.TextUnformatted("Draw glyphs on the surface.\n\nUse the left-mouse button to draw.\n\nThe right-mouse button changes the current pencil tip to the foreground, background, and glyph, that is under the cursor.");
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }

        ImGuiWidgets.BeginGroupPanel("Settings");

        if (ImGui.BeginTable("table1", 2))
        {
            ImGui.TableSetupColumn("one", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("two");

            System.Numerics.Vector4 color = _tip.Foreground.ToVector4();

            // Foreground
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Foreground:");

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGui.ColorEdit4("##fore", ref color, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs))
                _tip.Foreground = color.ToColor();

            // Background
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            color = _tip.Background.ToVector4();
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Background:");

            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGui.ColorEdit4("##back", ref color, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs))
                _tip.Background = color.ToColor();

            // Glyph
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Glyph:");

            ImGui.TableSetColumnIndex(1);

            var surface = (ImGuiCore.State.GetOpenDocument() as IDocumentSurface).Surface;

            int glyph = _tip.Glyph;
            if (ImGui.InputInt("##glyphinput", ref glyph, 1))
            {
                _tip.Glyph = Math.Clamp(glyph, 0, surface.Font.TotalGlyphs - 1);
            }
            ImGui.SameLine();


            var fontTexture = renderer.BindTexture(((Host.GameTexture)surface.Font.Image).Texture);
            var rect = surface.Font.GetGlyphSourceRectangle(_tip.Glyph);
            var textureSize = new SadRogue.Primitives.Point(surface.Font.Image.Width, surface.Font.Image.Height);

            var renderAreaSize = surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2();

            if (ImGui.ImageButton(fontTexture,
                                  renderAreaSize,
                                  rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize),
                                  0, _tip.Background.ToVector4(), _tip.Foreground.ToVector4()))
            {
                ImGui.OpenPopup("glyph_select");
            }

            if (ImGui.BeginPopup("glyph_select"))
            {
                ImGui.Text("Select your glyph");
                ImGui.EndPopup();
            }

            ImGui.EndTable();
        }
        ImGuiWidgets.EndGroupPanel();
    }

    public void MouseOver(IScreenSurface surface, SadRogue.Primitives.Point hoveredCellPosition, ImGuiRenderer renderer)
    {
        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            surface.Surface[hoveredCellPosition].Clear();
            _tip.CopyAppearanceTo(surface.Surface[hoveredCellPosition]);
            surface.IsDirty = true;
        }
        else if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
        {
            surface.Surface[hoveredCellPosition].CopyAppearanceTo(_tip);
            
            surface.IsDirty = true;
        }
    }
}
