using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Editor.Documents;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Info : ITool
{
    public string Title => "\ue66a Info";

    public string Description => "Displays information about the glyph under the cursor.";

    public void BuildSettingsPanel(Document document)
    {
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {
        if (!isHovered) return;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 4f);
        ImGui.BeginTooltip();

        var fontTexture = ImGuiCore.Renderer.BindTexture(((Host.GameTexture)document.EditingSurfaceFont.Image).Texture);
        var rect = document.EditingSurfaceFont.GetGlyphSourceRectangle(document.EditingSurface.Surface[hoveredCellPosition].Glyph);
        var textureSize = new SadRogue.Primitives.Point(document.EditingSurfaceFont.Image.Width, document.EditingSurfaceFont.Image.Height);

        ImGui.Image(fontTexture, document.EditingSurfaceFont.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize),
            (rect.Position + rect.Size).ToUV(textureSize));
        var cursorPosForSecondFont = ImGui.GetCursorPos();
        ImGui.SameLine();
        var cursorPosForDetails = ImGui.GetCursorPos();
        ImGui.SetCursorPos(cursorPosForSecondFont);

        var rectSolid = document.EditingSurfaceFont.SolidGlyphRectangle;
        ImGui.Image(fontTexture, document.EditingSurfaceFont.GetFontSize(IFont.Sizes.Two).ToVector2(), rectSolid.Position.ToUV(textureSize),
            (rectSolid.Position + rectSolid.Size).ToUV(textureSize), document.EditingSurface.Surface[hoveredCellPosition].Background.ToVector4());
        ImGui.SetCursorPos(cursorPosForSecondFont);
        ImGui.Image(fontTexture, document.EditingSurfaceFont.GetFontSize(IFont.Sizes.Two).ToVector2(), rect.Position.ToUV(textureSize),
            (rect.Position + rect.Size).ToUV(textureSize), document.EditingSurface.Surface[hoveredCellPosition].Foreground.ToVector4());

        ImGui.SetCursorPos(cursorPosForDetails);

        ImGui.BeginGroup();

        ImGui.Text($"Glyph: {document.EditingSurface.Surface[hoveredCellPosition].Glyph}");
        ImGui.AlignTextToFramePadding();
        ImGui.Text($"Foreground: ");
        ImGui.SameLine();
        ImGui.ColorButton("surface_cell_color", document.EditingSurface.Surface[hoveredCellPosition].Foreground.ToVector4(),
                          ImGuiColorEditFlags.NoPicker);
        ImGui.AlignTextToFramePadding();
        ImGui.Text($"Background: ");
        ImGui.SameLine();
        ImGui.ColorButton("surface_cell_color", document.EditingSurface.Surface[hoveredCellPosition].Background.ToVector4(),
                          ImGuiColorEditFlags.NoPicker);

        ImGui.EndGroup();

        ImGui.EndTooltip();
        ImGui.PopStyleVar();
    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}
