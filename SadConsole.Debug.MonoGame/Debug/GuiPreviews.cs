using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;
using SadRogue.Primitives;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;

namespace SadConsole.Debug;

public class GuiPreviews : ImGuiObjectBase
{
    private int _scOutputMode = ModeNormal;
    private int _surfaceOutputMode = ModeNormal;

    private const int ModeNormal = 0;
    private const int Mode2X = 1;
    private const int ModeFit = 2;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.SetNextWindowClass(ref GuiDockspace.AutoHideTabBar);
        ImGui.SetNextWindowBgAlpha(1f);
        ImGui.Begin(GuiDockspace.ID_CENTER_PANEL, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
        {
            if (ImGui.BeginTabBar("preview_tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
            {
                if (ImGui.BeginTabItem("SadConsole Output", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
                {

                    if (SadConsole.Host.Global.RenderOutput != null)
                    {
                        ImTextureID texture = renderer.BindTexture(SadConsole.Host.Global.RenderOutput);

                        ImGui.RadioButton("Normal", ref _scOutputMode, ModeNormal);
                        ImGui.SameLine();
                        ImGui.RadioButton("2x", ref _scOutputMode, Mode2X);
                        ImGui.SameLine();
                        ImGui.RadioButton("Fit", ref _scOutputMode, ModeFit);

                        ImGui.BeginChild("output1", ImGuiWindowFlags.HorizontalScrollbar);
                        ImGuiSC.DrawTexture("output_preview_image", true, _scOutputMode, texture, new Vector2(Host.Global.RenderOutput.Width, Host.Global.RenderOutput.Height), out _, out _);
                        if (GuiState._hoveredScreenObjectState != null)
                        {
                            if (GuiState._hoveredScreenObjectState.IsScreenSurface)
                            {
                                IScreenSurface surface = (IScreenSurface)GuiState._hoveredScreenObjectState.Object;

                                var drawList = ImGui.GetWindowDrawList();
                                Vector2 itemRectMin = ImGui.GetItemRectMin();
                                Vector2 boxTopLeft = new(itemRectMin.X + surface.AbsoluteArea.X, itemRectMin.Y + surface.AbsoluteArea.Y);
                                Vector2 boxBottomRight = boxTopLeft + new Vector2(surface.AbsoluteArea.Width, surface.AbsoluteArea.Height);
                                drawList.AddRect(boxTopLeft, boxBottomRight, Color.Violet.PackedValue);
                            }
                        }
                        ImGui.EndChild();
                    }
                    else
                        ImGui.Text("Rendering output hasn't been created.");

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Item Preview", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
                {
                    // Render output texture
                    if (GuiState._selectedScreenObject is IScreenSurface surface)
                    {
                        if (surface.Renderer?.Output is Host.GameTexture gameTexture)
                        {
                            Texture2D targetTexture = gameTexture.Texture;

                            // List the render steps if more than one
                            if (GuiState._selectedScreenObjectState.SurfaceState.RenderSteps.Length > 2)
                            {
                                ImGui.AlignTextToFramePadding();
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


                            ImGui.RadioButton("Normal", ref _surfaceOutputMode, ModeNormal);
                            ImGui.SameLine();
                            ImGui.RadioButton("2x", ref _surfaceOutputMode, Mode2X);
                            ImGui.SameLine();
                            ImGui.RadioButton("Fit", ref _surfaceOutputMode, ModeFit);

                            // Render the target texture
                            ImGui.BeginChild("output2", ImGuiWindowFlags.HorizontalScrollbar);
                            ImGuiSC.DrawTexture("output_preview_surface1", true, _surfaceOutputMode, targetTexture, renderer, out var isActive, out var isHovered);

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
                            ImGui.EndChild();
                        }
                        else
                            ImGui.Text("Selected object doesn't have a renderer");
                    }
                    else
                        ImGui.Text("Selected object isn't IScreenSurface");
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
        ImGui.End();
    }
}
