using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;
using SadConsole.Host;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

public static class SimpleObjectHelpers
{
    public static bool ImGuiDrawObjectsList(Document document, [NotNullWhen(true)] out SimpleObjectDefinition? obj)
    {
        IDocumentSimpleObjects docSimpleObjects = (IDocumentSimpleObjects)document;

        if (ImGui.Button("Manage Objects"u8))
            new Windows.SimpleObjectEditor(docSimpleObjects.SimpleObjects, document.EditingSurface.Surface.DefaultForeground.ToVector4(), document.EditingSurface.Surface.DefaultBackground.ToVector4(), document.EditingSurfaceFont).Open();

        ImGui.SetNextItemWidth(-1);

        if (ImGui.BeginListBox("##pencil_simplegameobjects"u8))
        {
            DrawSelectables("pencil_simplegameobjects", docSimpleObjects.SimpleObjects, document.EditingSurfaceFont);

            ImGui.EndListBox();
        }

        bool isItemSelected = docSimpleObjects.SimpleObjects.IsItemSelected();

        if (isItemSelected)
        {
            ImGui.SeparatorText("Selected Object"u8);

            ImGuiSC.FontGlyph.Draw(ImGuiCore.Renderer, "gameobject_definition",
                document.EditingSurfaceFont,
                docSimpleObjects.SimpleObjects.SelectedItem!.Visual);
            ImGui.SameLine();
            ImGui.Text(docSimpleObjects.SimpleObjects.SelectedItem!.ToString());

            obj = docSimpleObjects.SimpleObjects.SelectedItem;

            return true;
        }

        obj = null;
        return false;
    }

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
