using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Tools;
internal class Operations : ITool
{
    private bool _onlyVisible;

    public string Name => "Surface Operations";

    public string Description => "Perform operations on the surface, such as shifting or clearing.";

    private ColoredGlyph _tip = SharedToolSettings.Tip;

    public void BuildSettingsPanel(ImGuiRenderer renderer)
    {
        ImGui.Checkbox("Only on visible area", ref _onlyVisible);

        ImGuiWidgets.BeginGroupPanel("Shift");

        ISurface surfaceArea;
        IScreenSurface doc = ImGuiCore.State.GetOpenDocument().VisualDocument;

        if (_onlyVisible)
            surfaceArea = ImGuiCore.State.GetOpenDocument().VisualDocument.Surface.GetSubSurface(ImGuiCore.State.GetOpenDocument().VisualDocument.Surface.View);
        else
            surfaceArea = ImGuiCore.State.GetOpenDocument().VisualDocument.Surface;

        if (ImGui.Button("Left"))
        {
            surfaceArea.ShiftLeft();
            doc.IsDirty = true;
        }
        else if (ImGui.Button("Right"))
        {
            surfaceArea.ShiftRight();
            doc.IsDirty = true;

        }
        else if (ImGui.Button("Up"))
        {
            surfaceArea.ShiftUp();
            doc.IsDirty = true;
        }
        else if (ImGui.Button("Down"))
        {
            surfaceArea.ShiftDown();
            doc.IsDirty = true;
        }

        ImGuiWidgets.EndGroupPanel();
    }

    public void MouseOver(Document document, Point hoveredCellPosition, bool isActive, ImGuiRenderer renderer)
    {
        
    }

    public void OnSelected() { }

    public void OnDeselected() { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document, ImGuiRenderer renderer) { }
}
