using System;
using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem;

public abstract class ImGuiWindow : ImGuiObjectBase
{
    public string Title { get; set; } = "";

    public bool IsOpen;

    public event EventHandler Closed;

    public bool RemoveOnClose = true;

    public bool DialogResult;

    public void Close()
    {
        IsOpen = false;
        OnClosed();
        Closed?.Invoke(this, EventArgs.Empty);
    }

    protected abstract void OnClosed();

    public static bool DrawButtons(out bool result, bool acceptDisabled = false, string cancelButtonText = "Cancel", string acceptButtonText = "Accept")
    {
        bool buttonClicked = false;
        result = false;

        // Cancel Button
        if (ImGui.Button(cancelButtonText))
            buttonClicked = true;

        // Accept Button -- Right-aligned
        float pos = ImGui.CalcTextSize(acceptButtonText).X + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().FramePadding.X * 2 + 2;
        ImGui.SameLine(ImGui.GetWindowWidth() - pos);
        ImGui.BeginDisabled(acceptDisabled);
        
        if (ImGui.Button(acceptButtonText))
        {
            buttonClicked = true;
            result = true;
        }
        ImGui.EndDisabled();

        return buttonClicked;
    }
}
