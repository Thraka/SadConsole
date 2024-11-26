using System;
using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem;

public abstract class ImGuiWindow : ImGuiObjectBase
{
    public string Title { get; set; } = "";

    public bool IsOpen;

    public event EventHandler Closed;

    public bool DialogResult;

    protected void OnClosed() =>
        Closed?.Invoke(this, EventArgs.Empty);


    public static bool DrawButtons(out bool result, bool acceptDisabled = false)
    {
        bool buttonClicked = false;
        result = false;

        ImGui.Separator();

        if (ImGui.Button("Cancel")) { buttonClicked = true; }

        // Right-align button
        float pos = ImGui.CalcTextSize("Accept").X + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().FramePadding.X * 2 + 2;
        ImGui.SameLine(ImGui.GetWindowWidth() - pos);
        ImGui.BeginDisabled(acceptDisabled);
        
        if (ImGui.Button("Accept"))
        {
            buttonClicked = true;
            result = true;
        }
        ImGui.EndDisabled();

        return buttonClicked;
    }
}
