using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.GuiParts.Tools;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;

internal class Pencil : ITool
{
    public string Name => "Pencil";

    public string Description => """
        Draw glyphs on the surface.
        
        Use the left-mouse button to draw.
        
        The right-mouse button changes the current pencil tip to the foreground, background, and glyph, that is under the cursor.
        """;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGuiWidgets.BeginGroupPanel("Settings");

        Vector4 foreground = SharedToolSettings.Tip.Foreground.ToVector4();
        Vector4 background = SharedToolSettings.Tip.Background.ToVector4();
        int glyph = SharedToolSettings.Tip.Glyph;
        Mirror mirror = SharedToolSettings.Tip.Mirror;
        IScreenSurface surface = ImGuiCore.State.GetOpenDocument().VisualDocument;

        SettingsTable.DrawCommonSettings("pencilsettings", true, true, true, true, true,
                                         ref foreground, surface.Surface.DefaultForeground.ToVector4(),
                                         ref background, surface.Surface.DefaultBackground.ToVector4(),
                                         ref mirror,
                                         ref glyph, surface.Font, renderer);

        SharedToolSettings.Tip.Foreground = foreground.ToColor();
        SharedToolSettings.Tip.Background = background.ToColor();
        SharedToolSettings.Tip.Mirror = mirror;
        SharedToolSettings.Tip.Glyph = glyph;

        ImGuiWidgets.EndGroupPanel();
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        if (ImGuiCore.State.IsPopupOpen) return;

        ToolHelpers.HighlightCell(hoveredCellPosition, document.VisualDocument.Surface.ViewPosition, document.VisualDocument.FontSize, Color.Green);

        if (!isActive) return;

        if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            document.VisualDocument.Surface[hoveredCellPosition].Clear();
            SharedToolSettings.Tip.CopyAppearanceTo(document.VisualDocument.Surface[hoveredCellPosition]);
            document.VisualDocument.IsDirty = true;
        }
        else if (ImGui.IsMouseDown(ImGuiMouseButton.Right))
        {
            document.VisualDocument.Surface[hoveredCellPosition].CopyAppearanceTo(SharedToolSettings.Tip);
        }
    }

    public void OnSelected() { }

    public void OnDeselected() { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer) { }
}
