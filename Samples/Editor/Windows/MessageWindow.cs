using ImGuiNET;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class MessageWindow : ImGuiWindow
{
    private string _message;

    public MessageWindow(string message)
    {
        Title = "Unable to save file";
        _message = message;
    }

    public void Show()
    {
        IsOpen = true;

        if (!ImGuiCore.GuiComponents.Contains(this))
            ImGuiCore.GuiComponents.Add(this);
    }


    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiExt.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.TextWrapped(_message);

                // Right-align button
                float pos = ImGui.CalcTextSize("Close").X + ImGui.GetStyle().ItemSpacing.X * 2;
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - pos);
                if (ImGui.Button("Close")) { DialogResult = false; IsOpen = false; }
                ImGui.EndPopup();
            }
        }
        else
        {
            OnClosed();
            ImGuiCore.GuiComponents.Remove(this);
        }
    }
}
