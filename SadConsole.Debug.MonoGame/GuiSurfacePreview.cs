using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;

namespace SadConsole.Debug.MonoGame
{
    class GuiSurfacePreview : ImGuiObjectBase
    {
        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (GuiState.ShowSurfacePreview)
            {
                ImGui.SetNextWindowBgAlpha(1f);
                ImGui.SetNextWindowSizeConstraints(new Vector2(200, 200), new Vector2(10000, 10000));
                if (ImGui.Begin("Surface preview", ref GuiState.ShowSurfacePreview, ImGuiWindowFlags.HorizontalScrollbar))
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

                    // Render output texture
                    if (GuiState._selectedScreenObject is IScreenSurface surface)
                    {
                        if (surface.Renderer?.Output is Host.GameTexture gameTexture)
                        {
                            var texture = renderer.BindTexture(gameTexture.Texture);
                            //var drawPointer = ImGui.GetBackgroundDrawList();
                            //var startPos = ImGui.GetWindowPos();
                            var textureSize = new Vector2(gameTexture.Width, gameTexture.Height);

                            //drawPointer.AddRectFilled(startPos, startPos + ImGui.GetWindowSize(), ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));

                            ImGui.Image(texture, textureSize, Vector2.Zero, Vector2.One, Vector4.One, Vector4.One);
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
