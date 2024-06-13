﻿using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;
internal class Info : ITool
{
    public string Name => "Info";

    public string Description => "Displays information about the glyph under the cursor.";

    private ColoredGlyph _tip = SharedToolSettings.Tip;

    public bool IsHoverShowing;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
    }

    public void MouseOver(IScreenSurface surface, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        Vector2 textureArea = ImGui.GetItemRectSize();
        Vector2 itemRectMin = ImGui.GetItemRectMin();

        // Fix this up...
        if (ImGui.IsMouseHoveringRect(itemRectMin, itemRectMin + textureArea))//new Vector2(gameTexture.Texture.Width, gameTexture.Texture.Height)))
                                                                              //if (ImGui.IsMouseHoveringRect(pos1, pos1 + new Vector2(gameTexture.Texture.Width, gameTexture.Texture.Height)))
        {
            IsHoverShowing = true;
            ImGui.OpenPopup("surface_cell");

            Vector2 mousePosition = ImGui.GetMousePos();
            Vector2 pos = mousePosition - ImGui.GetItemRectMin();
            //if (surface.AbsoluteArea.WithPosition((0, 0)).Contains(new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y)))
            {
                //SadRogue.Primitives.Point cellPosition = new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(surface.FontSize) + surface.Surface.ViewPosition;
                Point cellPosition = hoveredCellPosition;
                // TODO: Keep this on the screen
                ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 4f);
                ImGui.BeginTooltip();

                var fontTexture = renderer.BindTexture(((Host.GameTexture)surface.Font.Image).Texture);
                var rect = surface.Font.GetGlyphSourceRectangle(surface.Surface[cellPosition].Glyph);
                var textureSize = new SadRogue.Primitives.Point(surface.Font.Image.Width, surface.Font.Image.Height);

                ImGui.Image(fontTexture, surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize));
                var cursorPosForSecondFont = ImGui.GetCursorPos();
                ImGui.SameLine();
                var cursorPosForDetails = ImGui.GetCursorPos();
                ImGui.SetCursorPos(cursorPosForSecondFont);

                var rectSolid = surface.Font.SolidGlyphRectangle;
                ImGui.Image(fontTexture, surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2(), rectSolid.Position.ToUV(textureSize), (rectSolid.Position + rectSolid.Size).ToUV(textureSize), surface.Surface[cellPosition].Background.ToVector4());
                ImGui.SetCursorPos(cursorPosForSecondFont);
                ImGui.Image(fontTexture, surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize), surface.Surface[cellPosition].Foreground.ToVector4());

                ImGui.SetCursorPos(cursorPosForDetails);
                ImGui.BeginGroup();
                {
                    ImGui.Text($"Glyph: {surface.Surface[cellPosition].Glyph}");
                    ImGui.Text($"Foreground: ");
                    ImGui.SameLine();
                    ImGui.ColorButton("surface_cell_color", surface.Surface[cellPosition].Foreground.ToVector4(), ImGuiColorEditFlags.NoPicker);
                    ImGui.Text($"Background: ");
                    ImGui.SameLine();
                    ImGui.ColorButton("surface_cell_color", surface.Surface[cellPosition].Background.ToVector4(), ImGuiColorEditFlags.NoPicker);
                }
                ImGui.EndGroup();

                ImGui.EndTooltip();
                ImGui.PopStyleVar();
            }
        }
    }

    public void OnSelected() { }

    public void OnDeselected() { }

    public void DocumentViewChanged() { }

    public void DrawOverDocument() { }
}
