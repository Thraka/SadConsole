using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;
internal class Operations : ITool
{
    public string Name => "Surface Operations";

    public string Description => "Perform operations on the surface, such as shifting or clearing.";

    private ColoredGlyph _tip = SharedToolSettings.Tip;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        //IScreenSurface surface = ImGuiCore.State.GetOpenDocument().Surface;
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        
    }

    public void OnSelected() { }

    public void OnDeselected() { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer) { }
}
