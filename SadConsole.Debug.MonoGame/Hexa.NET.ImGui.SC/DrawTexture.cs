using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using SadConsole.ImGuiSystem;

namespace Hexa.NET.ImGui.SC;

public static partial class ImGuiSC
{
    public static uint Color_White = ImGui.GetColorU32(SadRogue.Primitives.Color.White.ToVector4());

    public static void DrawTexture(string id, bool border, int zoomMode, Texture2D texture, ImGuiRenderer renderer, out bool isItemActive, out bool isItemHovered) =>
        DrawTexture(id, border, zoomMode, renderer.BindTexture(texture), new Vector2(texture.Width, texture.Height), ImGui.GetContentRegionAvail(), out isItemActive, out isItemHovered);

    public static void DrawTexture(string id, bool border, int zoomMode, Texture2D texture, Vector2 region, ImGuiRenderer renderer, out bool isItemActive, out bool isItemHovered) =>
        DrawTexture(id, border, zoomMode, renderer.BindTexture(texture), new Vector2(texture.Width, texture.Height), region, out isItemActive, out isItemHovered);

    public static void DrawTexture(string id, bool border, int zoomMode, ImTextureID texture, Vector2 textureSize, out bool isItemActive, out bool isItemHovered) =>
        DrawTexture(id, border, zoomMode, texture, textureSize, ImGui.GetContentRegionAvail(), out isItemActive, out isItemHovered);

    public static void DrawTexture(string id, bool border, int zoomMode, ImTextureID texture, Vector2 textureSize, Vector2 region, out bool isItemActive, out bool isItemHovered)
    {
        ImGui.PushID(id);
        Vector2 startPos = ImGui.GetCursorScreenPos();

        if (zoomMode == Zoom2x)
            textureSize *= 2;
        else if (zoomMode == ZoomFit)
            textureSize = region;

        ImGui.InvisibleButton("##imagebutton", textureSize, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight);

        isItemActive = ImGui.IsItemActive();
        isItemHovered = ImGui.IsItemHovered();

        //ImGui.SetCursorScreenPos(startPos);
        //ImGui.Image(texture, textureSize, Vector2.Zero, Vector2.One);

        var drawPointer = ImGui.GetWindowDrawList();
        drawPointer.AddImage(texture, startPos, startPos + textureSize, Vector2.Zero, Vector2.One, ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)));
        if (border)
        {
            drawPointer.AddRect(startPos, startPos + textureSize, Color_White);
        }
        ImGui.PopID();
    }
}
