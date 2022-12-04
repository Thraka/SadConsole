using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace SadConsole.ImGuiSystem
{
    public static partial class ImGuiExt
    {
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint igGetActiveID();

        public unsafe static uint GetActiveID()
        {
            return igGetActiveID();
        }

        internal unsafe static string StringFromPtr(byte* ptr)
        {
            int i;
            for (i = 0; ptr[i] != 0; i++)
            {
            }

            return Encoding.UTF8.GetString(ptr, i);
        }

        public static int ZoomNormal = 0;
        public static int Zoom2x = 1;
        public static int ZoomFit = 2;
        
        public static uint Color_White = ImGui.GetColorU32(SadRogue.Primitives.Color.White.ToVector4());

        public static void DrawTextureChild(string id, bool border, int zoomMode, Texture2D texture, ImGuiRenderer renderer, out bool isItemActive, out bool isItemHovered) =>
            DrawTextureChild(id, border, zoomMode, renderer.BindTexture(texture), new Vector2(texture.Width, texture.Height), ImGui.GetContentRegionAvail(), out isItemActive, out isItemHovered);

        public static void DrawTextureChild(string id, bool border, int zoomMode, Texture2D texture, Vector2 region, ImGuiRenderer renderer, out bool isItemActive, out bool isItemHovered) =>
            DrawTextureChild(id, border, zoomMode, renderer.BindTexture(texture), new Vector2(texture.Width, texture.Height), region, out isItemActive, out isItemHovered);

        public static void DrawTextureChild(string id, bool border, int zoomMode, IntPtr texture, Vector2 textureSize, out bool isItemActive, out bool isItemHovered) =>
            DrawTextureChild(id, border, zoomMode, texture, textureSize, ImGui.GetContentRegionAvail(), out isItemActive, out isItemHovered);

        public static void DrawTextureChild(string id, bool border, int zoomMode, IntPtr texture, Vector2 textureSize, Vector2 region, out bool isItemActive, out bool isItemHovered)
        {
            ImGui.PushID(id);
            //ImGui.BeginChild(id, region, false, ImGuiWindowFlags.NoScrollbar);
            {
                var startPos = ImGui.GetCursorScreenPos();

                if (zoomMode == Zoom2x)
                    textureSize *= 2;
                else if (zoomMode == ZoomFit)
                    textureSize = region;

                ImGui.InvisibleButton("##imagebutton", textureSize, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight);

                isItemActive = ImGui.IsItemActive();
                isItemHovered = ImGui.IsItemHovered(ImGuiHoveredFlags.RectOnly);

                //ImGui.SetCursorScreenPos(startPos);
                //ImGui.Image(texture, textureSize, Vector2.Zero, Vector2.One);
                var drawPointer = ImGui.GetWindowDrawList();
                drawPointer.AddImage(texture, startPos, startPos + textureSize, Vector2.Zero, Vector2.One, ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)));
                if (border)
                {
                    drawPointer.AddRect(startPos, startPos + textureSize, Color_White);
                }
            }
            //ImGui.EndChild();
            ImGui.PopID();
        }

        public static void CenterNextWindow() =>
            ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2f, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));

        #region VSliderIntNudges

        public static bool VSliderIntNudges(string label, Vector2 size, ref int v, int v_min, int v_max) =>
            VSliderIntNudges(label, size, ref v, v_min, v_max, "%d", ImGuiSliderFlags.None);

        public static bool VSliderIntNudges(string label, Vector2 size, ref int v, int v_min, int v_max, ImGuiSliderFlags flags) =>
            VSliderIntNudges(label, size, ref v, v_min, v_max, "%d", flags);

        public static bool VSliderIntNudges(string label, Vector2 size, ref int v, int v_min, int v_max, string fmt, ImGuiSliderFlags flags)
        {
            bool returnValue = false;

            // Calculate button size, taken from widgets.cpp in imgui lib
            var style = ImGui.GetStyle();
            Vector2 label_size = ImGui.CalcTextSize("+", 0, true) + (style.FramePadding * 2f);
            ImGui.BeginChild(label, size);

            if (ImGui.Button($"-", new Vector2(size.X, label_size.Y)))
            {
                returnValue = true;
                v = Math.Clamp(v - 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
            }

            returnValue = returnValue | ImGui.VSliderInt("##vslider", new Vector2(size.X, size.Y - (style.FramePadding.Y * 4f) - (label_size.Y * 2)), ref v, v_min, v_max, fmt, flags);

            if (ImGui.Button("+", new Vector2(size.X, label_size.Y)))
            {
                returnValue = true;
                v = Math.Clamp(v + 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
            }

            ImGui.EndChild();

            return returnValue;
        }

        #endregion

        #region SliderIntNudges

        public static bool SliderIntNudges(string label, int width, ref int v, int v_min, int v_max) =>
            SliderIntNudges(label, width, ref v, v_min, v_max, "%d", ImGuiSliderFlags.None);

        public static bool SliderIntNudges(string label, int width, ref int v, int v_min, int v_max, ImGuiSliderFlags flags) =>
            SliderIntNudges(label, width, ref v, v_min, v_max, "%d", flags);

        public static bool SliderIntNudges(string label, int width, ref int v, int v_min, int v_max, string fmt, ImGuiSliderFlags flags)
        {
            bool returnValue = false;

            // Calculate button size, taken from widgets.cpp in imgui lib
            var style = ImGui.GetStyle();

            ImGui.BeginChild(label, new Vector2(width, 0));

            if (ImGui.Button("-"))
            {
                returnValue = true;
                v = Math.Clamp(v - 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
            }

            ImGui.SameLine();

            var buttonWidth = ImGui.GetCursorPosX();

            ImGui.SetNextItemWidth(width - (buttonWidth * 2));

            returnValue = returnValue | ImGui.SliderInt("##width", ref v, v_min, v_max, fmt, flags);

            ImGui.SameLine();

            if (ImGui.Button("+", new Vector2()))
            {
                returnValue = true;
                v = Math.Clamp(v + 1, Math.Min(v_min, v_max), Math.Max(v_min, v_max));
            }

            ImGui.EndChild();

            return returnValue;
        }

        #endregion

        public static bool ScrollableSurface(string id, IScreenSurface Surface, out SadRogue.Primitives.Point hoveredCellPosition, ImGuiRenderer renderer)
        {
            var region = ImGui.GetContentRegionAvail();
            var imageSize = new Vector2(Surface.Renderer.Output.Width, Surface.Renderer.Output.Height);
            var barSize = 15;
            var padding = ImGui.GetStyle().FramePadding;
            bool bigX = false;
            bool returnValue = false;

            
            var newViewWidth = (int)(region.X - barSize - (padding.X * 2)) / Surface.FontSize.X;
            var newViewHeight = (int)(region.Y - barSize - 2 - (padding.Y * 2)) / Surface.FontSize.Y; // minus 2 is here because of button height

            newViewWidth = Math.Max(newViewWidth, 1);
            newViewHeight = Math.Max(newViewHeight, 1);

            if (Surface.Surface.Width < newViewWidth && Surface.Surface.Width != Surface.Surface.ViewWidth)
                Surface.Surface.ViewWidth = Surface.Surface.Width;
            else if (Surface.Surface.Width > newViewWidth)
                Surface.Surface.ViewWidth = newViewWidth;

            if (Surface.Surface.Height < newViewHeight && Surface.Surface.Height != Surface.Surface.ViewHeight)
                Surface.Surface.ViewHeight = Surface.Surface.Height;
            else if (Surface.Surface.Height > newViewHeight)
                Surface.Surface.ViewHeight = newViewHeight;

            hoveredCellPosition = new SadRogue.Primitives.Point();

            ImGui.BeginChild(id);
            //isActive = ImGui.IsItemActive();

            //if (idother != 0 && idvalue != idother) System.Diagnostics.Debugger.Break();

            DrawTextureChild("output_preview_surface1", true, ImGuiExt.ZoomNormal, ((SadConsole.Host.GameTexture)Surface.Renderer.Output).Texture, imageSize, renderer, out var isActive, out var isHovered);

            Vector2 mousePosition = ImGui.GetMousePos();
            Vector2 pos = mousePosition - ImGui.GetItemRectMin();
            if (Surface.AbsoluteArea.WithPosition((0, 0)).Contains(new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y)))
            {
                if (isActive && isHovered)
                {
                    hoveredCellPosition = new SadRogue.Primitives.Point((int)pos.X, (int)pos.Y).PixelLocationToSurface(Surface.FontSize) + Surface.Surface.ViewPosition;
                    returnValue = true;
                }
            }

            var view = Surface.Surface.View;

            if (view.Height != Surface.Surface.Height)
            {
                ImGui.SameLine();

                int _sliderValueY = view.Position.Y;

                if (ImGuiExt.VSliderIntNudges("##height", new Vector2(barSize, imageSize.Y), ref _sliderValueY, Surface.Surface.Height - view.Height, 0, ImGuiSliderFlags.AlwaysClamp))
                    Surface.Surface.ViewPosition = Surface.Surface.ViewPosition.WithY(_sliderValueY);
            }

            if (view.Width != Surface.Surface.Width)
            {
                int _sliderValueX = view.Position.X;

                if (ImGuiExt.SliderIntNudges("##width", (int)imageSize.X, ref _sliderValueX, 0, Surface.Surface.Width - view.Width, bigX ? "BIG" : "%d", ImGuiSliderFlags.AlwaysClamp))
                    Surface.Surface.ViewPosition = Surface.Surface.ViewPosition.WithX(_sliderValueX);
            }

            ImGui.EndChild();

            return returnValue;
        }
    }
}
