using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;
using SadConsole.Numerics;
using SadConsole.ImGuiSystem;

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
                if (ImGui.Begin("Surface preview", ref GuiState.ShowSurfacePreview))
                {
                    // TODO:
                    // Change this to have a list box that presents the render steps
                    // Select render step checks for IRenderTexture
                    //   - Display selected texture
                    // Select render step checks if selected is Output
                    //   - Display final texture

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
                            // Add options
                            //ImGui.Checkbox("Zoom", ref _zoom);
                            ImGuiExt.DrawTextureChild("output_preview_surface1", true, _zoom ? ImGuiExt.Zoom2x : ImGuiExt.ZoomNormal, gameTexture.Texture, renderer);
                            Vector2 mousePosition = ImGui.GetMousePos();
                            Vector2 pos = mousePosition - ImGui.GetItemRectMin();
                            if (surface.AbsoluteArea.WithPosition((0,0)).Contains(new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y)))
                            {
                                SadRogue.Primitives.Point cellPosition = new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(surface.FontSize) + surface.Surface.ViewPosition;

                                if (ImGui.IsItemHovered())
                                    ImGui.OpenPopup("surface_cell");

                                // TODO: Keep this on the screen
                                ImGui.SetNextWindowPos(mousePosition + new Vector2(10f, 10f));

                                ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 4f);
                                
                                if (ImGui.BeginPopup("surface_cell"))
                                {
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
                                    ImGui.EndPopup();
                                }
                                ImGui.PopStyleVar();
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
