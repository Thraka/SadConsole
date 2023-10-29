using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts;

public class SurfacePreview : ImGuiWindow
{
    private bool _windowOpenState;
    private bool _zoom;
    private IScreenSurface _surface;

    public SurfacePreview(IScreenSurface surface)
    {
        Title = "Surface";
        _surface = surface;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGuiExt.CenterNextWindow();
            //ImGui.SetNextWindowSizeConstraints(new System.Numerics.Vector2(350, 350), new System.Numerics.Vector2(-1, -1));
            ImGui.SetNextWindowBgAlpha(1f);
            if (ImGui.Begin(Title, ref IsOpen, ImGuiWindowFlags.AlwaysHorizontalScrollbar))
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
                if (_surface.Renderer?.Output is Host.GameTexture gameTexture)
                {
                    // Add options
                    //ImGui.Checkbox("Zoom", ref _zoom);
                    bool isShowingHover = false;
                    var pos1 = ImGui.GetCursorPos();
                    
                    ImGuiExt.DrawTextureChild("output_preview_surface1", true, _zoom ? ImGuiExt.Zoom2x : ImGuiExt.ZoomNormal, gameTexture.Texture, renderer, out _, out _);
                    var textureArea = ImGui.GetItemRectSize();
                    var innerRegion = ImGui.GetWindowContentRegionMax();
                    var itemRectMin = ImGui.GetItemRectMin();

                    // Fix this up...
                    if (ImGui.IsMouseHoveringRect(itemRectMin, itemRectMin + textureArea))//new Vector2(gameTexture.Texture.Width, gameTexture.Texture.Height)))
                        //if (ImGui.IsMouseHoveringRect(pos1, pos1 + new Vector2(gameTexture.Texture.Width, gameTexture.Texture.Height)))
                        {
                        isShowingHover = true;
                        ImGui.OpenPopup("surface_cell");
                        
                        Vector2 mousePosition = ImGui.GetMousePos();
                        Vector2 pos = mousePosition - ImGui.GetItemRectMin();
                        if (_surface.AbsoluteArea.WithPosition((0, 0)).Contains(new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y)))
                        {
                            SadRogue.Primitives.Point cellPosition = new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(_surface.FontSize) + _surface.Surface.ViewPosition;

                            // TODO: Keep this on the screen
                            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 4f);
                            ImGui.BeginTooltip();

                            var fontTexture = renderer.BindTexture(((Host.GameTexture)_surface.Font.Image).Texture);
                            var rect = _surface.Font.GetGlyphSourceRectangle(_surface.Surface[cellPosition].Glyph);
                            var textureSize = new SadRogue.Primitives.Point(_surface.Font.Image.Width, _surface.Font.Image.Height);

                            ImGui.Image(fontTexture, _surface.Font.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize), (rect.Position + rect.Size).ToUV(textureSize));
                            ImGui.SameLine();
                            ImGui.SameLine();
                            ImGui.BeginGroup();
                            {
                                ImGui.Text($"Glyph: {_surface.Surface[cellPosition].Glyph}");
                                ImGui.Text($"Foreground: ");
                                ImGui.SameLine();
                                ImGui.ColorButton("surface_cell_color", _surface.Surface[cellPosition].Foreground.ToVector4(), ImGuiColorEditFlags.NoPicker);
                                ImGui.Text($"Background: ");
                                ImGui.SameLine();
                                ImGui.ColorButton("surface_cell_color", _surface.Surface[cellPosition].Background.ToVector4(), ImGuiColorEditFlags.NoPicker);
                            }
                            ImGui.EndGroup();

                            ImGui.EndTooltip();
                            ImGui.PopStyleVar();
                        }
                    }
                    ImGui.SetCursorPos(pos1 + new Vector2(0, gameTexture.Texture.Height));
                    ImGui.Text("Show: " + isShowingHover);
                    ImGui.Text("InnerRegion: " + innerRegion);
                    ImGui.Text("TextureArea: " + textureArea);
                    ImGui.Text("MinRect: " + itemRectMin);
                }
            }
            ImGui.End();
        }
    }
}
