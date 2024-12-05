using System.Collections.Generic;
using System;
using System.Numerics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using SadConsole;
using System.Linq;

namespace Hexa.NET.ImGui;

public static partial class ImGui2
{
    public static class GlyphPickerPopup
    {
        public static bool Show(ImGuiRenderer renderer, string popupId, IFont font, ImTextureID fontTexture, Point fontTextureSize, ref int selectedGlyph)
        {
            bool returnValue = false;

            if (ImGui.BeginPopup(popupId))
            {
                // Get shared space to reserve font icon preview
                Vector2 size = ImGui.CalcTextSize("Glyph: None");
                size.Y = Math.Max(size.Y, font.GlyphHeight);

                // Draw font image
                ImGui.Text("Select your glyph");
                ImGui2.DrawTexture("font_preview", true, ImGui2.ZoomNormal, ((SadConsole.Host.GameTexture)font.Image).Texture, renderer, out bool isActive, out bool isHovered);

                if (isHovered)
                {
                    Vector2 mousePosition = ImGui.GetMousePos();
                    Vector2 itemRectMin = ImGui.GetItemRectMin();
                    Vector2 itemRectMax = ImGui.GetItemRectMax();
                    //Vector2 itemRectSize = ImGui.GetItemRectSize();
                    Vector2 pos = mousePosition - itemRectMin;

                    KeyValuePair<int, Rectangle> glyphRect = ((SadFont)font).GlyphRectangles.Where(kv => kv.Value.Contains(new Point((int)pos.X, (int)pos.Y))).FirstOrDefault();

                    if (glyphRect.Value != Rectangle.Empty)
                        if (isActive)
                        {
                            ImGui.CloseCurrentPopup();
                            selectedGlyph = glyphRect.Key;
                            returnValue = true;
                        }
                        else
                        {
                            ImDrawListPtr drawList = ImGui.GetWindowDrawList();//->AddRect(labelMin, labelMax, IM_COL32(255, 0, 255, 255));

                            Vector2 boxTopLeft = new(itemRectMin.X + glyphRect.Value.X, itemRectMin.Y + glyphRect.Value.Y);
                            Vector2 boxBottomRight = boxTopLeft + new Vector2(glyphRect.Value.Width, glyphRect.Value.Height);

                            drawList.AddRect(boxTopLeft, boxBottomRight, Color.Violet.PackedValue);

                            float verticalLineX = boxTopLeft.X + glyphRect.Value.Width / 2;
                            float horizontalLineY = boxTopLeft.Y + glyphRect.Value.Height / 2;

                            //drawList.AddLine(new Vector2(verticalLineX, itemRectMin.Y), new Vector2(verticalLineX, boxTopLeft.Y), Color.Violet.PackedValue, 2);
                            //drawList.AddLine(new Vector2(verticalLineX, boxBottomRight.Y - 1), new Vector2(verticalLineX, itemRectMax.Y), Color.Violet.PackedValue, 2);
                            //drawList.AddLine(new Vector2(itemRectMin.X, horizontalLineY), new Vector2(boxTopLeft.X, horizontalLineY), Color.Violet.PackedValue, 2);
                            //drawList.AddLine(new Vector2(boxBottomRight.X - 1, horizontalLineY), new Vector2(itemRectMax.X, horizontalLineY), Color.Violet.PackedValue, 2);

                            ImGui.Text($"Glyph: {glyphRect.Key}");
                            ImGui.SameLine();
                            ImGui.Image(fontTexture,
                                            new Vector2(glyphRect.Value.Width, glyphRect.Value.Height),
                                            glyphRect.Value.Position.ToUV(fontTextureSize), (glyphRect.Value.Position + glyphRect.Value.Size).ToUV(fontTextureSize));
                        }
                    else
                    {
                        Vector2 cursor = ImGui.GetCursorPos();
                        ImGui.InvisibleButton("##padded-area-preview", size);
                        ImGui.SetCursorPos(cursor);
                        ImGui.Text($"Glyph: None");
                    }
                }
                else
                {
                    Vector2 cursor = ImGui.GetCursorPos();
                    ImGui.InvisibleButton("##padded-area-preview", size);
                    ImGui.SetCursorPos(cursor);
                    ImGui.Text($"Glyph: None");
                }

                ImGui.EndPopup();
            }

            return returnValue;
        }
    }

}
