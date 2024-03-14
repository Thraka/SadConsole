using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;

namespace SadConsole.Debug.MonoGame
{
    class GuiSurfacePreview : ImGuiObjectBase
    {
        private bool _windowOpenState;
        private bool _zoom;

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (GuiState.ShowSurfacePreview)
            {
                ImGui.SetNextWindowBgAlpha(1f);
                //ImGui.SetNextWindowSizeConstraints(new Vector2(200, 200), new Vector2(10000, 10000));
                if (ImGui.Begin("Surface preview", ref GuiState.ShowSurfacePreview, ImGuiWindowFlags.HorizontalScrollbar))
                {
                    // TODO:
                    // New window that is an editor type for the parent object
                    // Inspect components to see if entity renderer, controls ui, etc, enable different editors.
                    // Add ability to add those components.

                    
                    // Check for zoom:


                    // Render output texture
                    if (GuiState._selectedScreenObject is IScreenSurface surface)
                    {
                        if (surface.Renderer?.Output is Host.GameTexture gameTexture)
                        {
                            Texture2D targetTexture = gameTexture.Texture;

                            // List the render steps if more than one
                            if (GuiState._selectedScreenObjectState.SurfaceState.RenderSteps.Length > 2)
                            {
                                ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Render steps: ");
                                ImGui.SameLine();
                                ImGui.SetNextItemWidth(200);
                                ImGui.Combo("##Rendersteps", ref GuiState._selectedScreenObjectState.SurfaceState.RenderStepSelectedItem,
                                                             GuiState._selectedScreenObjectState.SurfaceState.RenderStepsNames,
                                                             GuiState._selectedScreenObjectState.SurfaceState.RenderStepsNames.Length);

                                if (GuiState._selectedScreenObjectState.SurfaceState.RenderStepSelectedItem != 0)
                                {
                                    targetTexture = ((Host.GameTexture)GuiState._selectedScreenObjectState.SurfaceState.RenderSteps[GuiState._selectedScreenObjectState.SurfaceState.RenderStepSelectedItem].CachedTexture).Texture;
                                }
                            }

                            // Render the target texture
                            ImGuiExt.DrawTextureChild("output_preview_surface1", false, _zoom ? ImGuiExt.Zoom2x : ImGuiExt.ZoomNormal, targetTexture, renderer, out var isActive, out var isHovered);

                            // Peek the cell if the target type is the final
                            if (GuiState._selectedScreenObjectState.SurfaceState.RenderStepSelectedItem == 0)
                            {
                                Vector2 mousePosition = ImGui.GetMousePos();
                                Vector2 pos = mousePosition - ImGui.GetItemRectMin();
                                if (surface.AbsoluteArea.WithPosition((0, 0)).Contains(new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y)))
                                {
                                    SadRogue.Primitives.Point cellPosition = new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(surface.FontSize) + surface.Surface.ViewPosition;

                                    if (ImGui.IsItemHovered())
                                    {
                                        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 4f);
                                        ImGui.BeginTooltip();

                                        var fontTexture = renderer.BindTexture(((Host.GameTexture)surface.Font.Image).Texture);
                                        var rect = surface.Font.GetGlyphSourceRectangle(surface.Surface[cellPosition].Glyph);
                                        var textureSize = new SadRogue.Primitives.Point(surface.Font.Image.Width, surface.Font.Image.Height);

                                        ImGui.Image(fontTexture, surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize));
                                        ImGui.SameLine();
                                        ImGui.SameLine();
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

                                    //if (ImGui.IsItemHovered())
                                    //    ImGui.OpenPopup("surface_cell");

                                    //// TODO: Keep this on the screen
                                    //ImGui.SetNextWindowPos(mousePosition + new Vector2(10f, 10f));

                                    //ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 4f);

                                    //// TODO: Popup shouldn't be used. The color editor has a tooltip presentation before you click that like this. Use that!
                                    //if (ImGui.BeginPopup("surface_cell"))
                                    //{
                                    //    var fontTexture = renderer.BindTexture(((Host.GameTexture)surface.Font.Image).Texture);
                                    //    var rect = surface.Font.GetGlyphSourceRectangle(surface.Surface[cellPosition].Glyph);
                                    //    var textureSize = new SadRogue.Primitives.Point(surface.Font.Image.Width, surface.Font.Image.Height);

                                    //    ImGui.Image(fontTexture, surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize));
                                    //    ImGui.SameLine();
                                    //    ImGui.SameLine();
                                    //    ImGui.BeginGroup();
                                    //    {
                                    //        ImGui.Text($"Glyph: {surface.Surface[cellPosition].Glyph}");
                                    //        ImGui.Text($"Foreground: ");
                                    //        ImGui.SameLine();
                                    //        ImGui.ColorButton("surface_cell_color", surface.Surface[cellPosition].Foreground.ToVector4(), ImGuiColorEditFlags.NoPicker);
                                    //        ImGui.Text($"Background: ");
                                    //        ImGui.SameLine();
                                    //        ImGui.ColorButton("surface_cell_color", surface.Surface[cellPosition].Background.ToVector4(), ImGuiColorEditFlags.NoPicker);
                                    //    }
                                    //    ImGui.EndGroup();
                                    //    ImGui.EndPopup();
                                    //}
                                    //ImGui.PopStyleVar();
                                }
                            }
                        }
                        else
                            ImGui.Text("Selected object doesn't have a renderer");
                    }
                    else
                        ImGui.Text("Selected object isn't IScreenSurface");
                }
                ImGui.End();
            }
        }
    }
}
