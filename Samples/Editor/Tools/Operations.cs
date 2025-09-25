using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;
using SadConsole.Editor.Documents;

namespace SadConsole.Editor.Tools;

internal class Operations : ITool
{
    private bool _onlyVisible;
    private bool _wrap;

    public string Title => "\uee1b Surface Operations";

    public string Description => "Perform operations on the surface, such as shifting or clearing.";

    private ColoredGlyph _tip = SharedToolSettings.Tip;

    public void BuildSettingsPanel(Document document)
    {
        ImGui.SeparatorText(Title);
        ImGui.Checkbox("Only on visible area", ref _onlyVisible);

        ImGuiSC.BeginGroupPanel("Shift");

        ImGui.Checkbox("Wrap Cells", ref _wrap);

        ISurface surfaceArea;
        ScreenSurface doc = document.EditingSurface;

        if (_onlyVisible)
            surfaceArea = document.EditingSurface.Surface.GetSubSurface(document.EditingSurface.Surface.View);
        else
            surfaceArea = document.EditingSurface.Surface;

        Vector2 buttonSize = new(ImGui.CalcTextSize("right").X + ImGui.GetStyle().FramePadding.X * 2, 0);

        if (ImGui.Button("Left", buttonSize))
        {
            surfaceArea.ShiftLeft(1, _wrap);
            doc.IsDirty = true;
        }

        ImGui.SameLine();
        if (ImGui.Button("Right", buttonSize))
        {
            surfaceArea.ShiftRight(1, _wrap);
            doc.IsDirty = true;

        }

        if (ImGui.Button("Up", buttonSize))
        {
            surfaceArea.ShiftUp(1, _wrap);
            doc.IsDirty = true;
        }

        ImGui.SameLine();
        if (ImGui.Button("Down", buttonSize))
        {
            surfaceArea.ShiftDown(1, _wrap);
            doc.IsDirty = true;
        }

        ImGuiSC.EndGroupPanel();
    }

    public void Process(Document document, Point hoveredCellPosition, bool isHovered, bool isActive)
    {

    }

    public void OnSelected(Document document) { }

    public void OnDeselected(Document document) { }

    public void Reset(Document document) { }

    public void DocumentViewChanged(Document document) { }

    public void DrawOverDocument(Document document) { }

    public override string ToString() =>
        Title;
}

