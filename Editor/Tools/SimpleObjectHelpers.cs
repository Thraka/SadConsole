using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Host;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

public static class SimpleObjectHelpers
{
    public static bool DrawSelectables(string id, ImGuiList<SimpleObjectDefinition> objects, IFont font)
    {
        bool itemSelected = false;

        ImDrawListPtr drawData = ImGui.GetWindowDrawList();
        ImTextureID fontTexture = ImGuiCore.Renderer.BindTexture(((GameTexture)font.Image).Texture);

        Vector2 outputSize = new(0, ImGui.GetFrameHeight() - ImGui.GetStyle().FramePadding.Y * 2);
        outputSize.X = (outputSize.Y / font.GlyphHeight) * font.GlyphWidth;

        for (int i = 0; i < objects.Count; i++)
        {
            var pos = ImGui.GetCursorPos();

            if (ImGui.Selectable($"##{id}{i}", objects.SelectedItemIndex == i, new Vector2(0, outputSize.Y)))
            {
                objects.SelectedItemIndex = i;
                itemSelected = true;
            }

            ImGui.SetCursorPos(pos);

            var rect = font.SolidGlyphRectangle;
            var textureSize = new Point(font.Image.Width, font.Image.Height);

            ImGui.Image(fontTexture, outputSize, rect.Position.ToUV(textureSize),
                (rect.Position + rect.Size).ToUV(textureSize), objects.Objects[i].Visual.Background.ToVector4());

            ImGui.SetCursorPos(pos);
            rect = font.GetGlyphSourceRectangle(objects.Objects[i].Visual.Glyph);

            ImGui.Image(fontTexture, outputSize, rect.Position.ToUV(textureSize),
                (rect.Position + rect.Size).ToUV(textureSize), objects.Objects[i].Visual.Foreground.ToVector4());

            ImGui.SameLine();
            ImGui.Text(objects.Objects[i].Name);
            ImGui.SameLine();

            ImGui.Dummy(new Vector2(0, outputSize.Y));
        }

        return itemSelected;
    }
}
