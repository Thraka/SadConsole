using System.Collections.Generic;
using System;
using System.Numerics;
using SadConsole.ImGuiSystem.Rendering;
using SadRogue.Primitives;
using SadConsole;
using System.Linq;

namespace Hexa.NET.ImGui;

public static partial class ImGuiSC
{
    public static class GlyphPickerPopup
    {
        private static int _hoveredGlyph = -1;
        private static KeyValuePair<int, Rectangle> _hoveredGlyphDefinition;

        public static bool Show(ImGuiRenderer renderer, string popupId, IFont font, ImTextureRef fontTexture, Point fontTextureSize, ref int selectedGlyph)
        {
            bool returnValue = false;

            if (ImGui.BeginPopup(popupId))
            {
                Rectangle glyphRect = ((SadFont)font).GetGlyphSourceRectangle(selectedGlyph);

                // Get shared space to reserve font icon preview
                Vector2 size = ImGui.CalcTextSize("Glyph: None"u8);
                size.Y = Math.Max(size.Y, font.GlyphHeight);

                // Draw font image
                ImGui.Text("Select your glyph"u8);
                ImGui.Text($"Current: {selectedGlyph}");
                ImGui.SameLine();
                ImGui.Image(fontTexture,
                                new Vector2(glyphRect.Width, glyphRect.Height),
                                glyphRect.Position.ToUV(fontTextureSize), (glyphRect.Position + glyphRect.Size).ToUV(fontTextureSize));

                ImGui.SameLine();
                if (_hoveredGlyph != -1)
                {
                    ImGui.Text($"Hovered: {_hoveredGlyph}");
                    ImGui.SameLine();
                    ImGui.Image(fontTexture,
                                new Vector2(_hoveredGlyphDefinition.Value.Width, _hoveredGlyphDefinition.Value.Height),
                                _hoveredGlyphDefinition.Value.Position.ToUV(fontTextureSize), (_hoveredGlyphDefinition.Value.Position + _hoveredGlyphDefinition.Value.Size).ToUV(fontTextureSize));
                }
                else
                {
                    ImGui.Text("Hovered: None"u8);
                }

                ImGuiSC.DrawTexture("font_preview", true, ImGuiSC.ZoomNormal, fontTexture, fontTextureSize.ToVector2(), out bool isActive, out bool isHovered);

                ImDrawListPtr drawList = ImGui.GetWindowDrawList();//->AddRect(labelMin, labelMax, IM_COL32(255, 0, 255, 255));

                Vector2 itemRectMin = ImGui.GetItemRectMin();
                Vector2 itemRectMax = ImGui.GetItemRectMax();

                Vector2 boxTopLeft = new(itemRectMin.X + glyphRect.X, itemRectMin.Y + glyphRect.Y);
                Vector2 boxBottomRight = boxTopLeft + new Vector2(glyphRect.Width, glyphRect.Height);

                drawList.AddRect(boxTopLeft, boxBottomRight, Color.Red.PackedValue);

                if (isHovered)
                {
                    Vector2 mousePosition = ImGui.GetMousePos();
                    Vector2 pos = mousePosition - itemRectMin;

                    _hoveredGlyphDefinition = ((SadFont)font).GlyphRectangles.First(kv => kv.Value.Contains(new Point((int)pos.X, (int)pos.Y)));

                    if (_hoveredGlyphDefinition.Value != Rectangle.Empty)
                    {
                        _hoveredGlyph = _hoveredGlyphDefinition.Key;

                        if (isActive)
                        {
                            ImGui.CloseCurrentPopup();
                            selectedGlyph = _hoveredGlyphDefinition.Key;
                            returnValue = true;
                        }
                        else
                        {
                            boxTopLeft = new(itemRectMin.X + _hoveredGlyphDefinition.Value.X, itemRectMin.Y + _hoveredGlyphDefinition.Value.Y);
                            boxBottomRight = boxTopLeft + new Vector2(_hoveredGlyphDefinition.Value.Width, _hoveredGlyphDefinition.Value.Height);

                            drawList.AddRect(boxTopLeft, boxBottomRight, Color.Violet.PackedValue);
                        }
                    }
                }

                ImGui.EndPopup();
            }

            return returnValue;
        }
    }

}
