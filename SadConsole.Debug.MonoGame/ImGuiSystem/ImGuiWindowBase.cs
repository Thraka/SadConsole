using System;
using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem;

public abstract class ImGuiWindowBase : ImGuiObjectBase
{
    public string Title { get; set; } = "";

    public bool IsOpen;

    public event EventHandler Closed;

    public bool RemoveOnClose = true;
    public bool AddOnOpen = true;
    public bool DialogResult;

    public void Close() =>
        OnClosed();

    protected virtual void OnClosed()
    {
        IsOpen = false;

        if (RemoveOnClose)
            ImGuiCore.GuiComponents.Remove(this);

        Closed?.Invoke(this, EventArgs.Empty);
    }

    public void Open() =>
        OnOpened();

    protected virtual void OnOpened()
    {
        IsOpen = true;

        if (AddOnOpen && !ImGuiCore.GuiComponents.Contains(this))
            ImGuiCore.GuiComponents.Add(this);
    }

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
