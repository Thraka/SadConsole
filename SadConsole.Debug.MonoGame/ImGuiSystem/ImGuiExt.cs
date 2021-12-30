using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.Numerics;

namespace SadConsole.ImGuiSystem
{
    public static class ImGuiExt
    {
        public static int ZoomNormal = 0;
        public static int Zoom2x = 1;
        public static int ZoomFit = 2;

        public static uint Color_White = ImGui.GetColorU32(SadRogue.Primitives.Color.White.ToVector4());

        public static void DrawTextureChild(string id, bool border, int zoomMode, Texture2D texture, ImGuiRenderer renderer) =>
            DrawTextureChild(id, border, zoomMode, renderer.BindTexture(texture), new Vector2(texture.Width, texture.Height), ImGui.GetContentRegionAvail());

        public static void DrawTextureChild(string id, bool border, int zoomMode, Texture2D texture, Vector2 region, ImGuiRenderer renderer) =>
            DrawTextureChild(id, border, zoomMode, renderer.BindTexture(texture), new Vector2(texture.Width, texture.Height), region);

        public static void DrawTextureChild(string id, bool border, int zoomMode, IntPtr texture, Vector2 textureSize) =>
            DrawTextureChild(id, border, zoomMode, texture, textureSize, ImGui.GetContentRegionAvail());

        public static void DrawTextureChild(string id, bool border, int zoomMode, IntPtr texture, Vector2 textureSize, Vector2 region)
        {
            ImGui.BeginChild(id, region, false, ImGuiWindowFlags.HorizontalScrollbar);
            {
                var startPos = ImGui.GetCursorScreenPos();

                if (zoomMode == Zoom2x)
                    textureSize *= 2;
                else if (zoomMode == ZoomFit)
                    textureSize = region;

                ImGui.Image(texture, textureSize, Vector2.Zero, Vector2.One);

                if (border)
                {
                    var drawPointer = ImGui.GetWindowDrawList();
                    drawPointer.AddRect(startPos, startPos + textureSize, Color_White);
                }
            }
            ImGui.EndChild();
        }


    }
}
