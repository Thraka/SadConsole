using SadConsole.ImGuiSystem;

namespace Hexa.NET.ImGui.SC.Windows;

public class MessageWindow : ImGuiWindowBase
{
    private string _message;

    public MessageWindow(string message, string title = "Message")
    {
        Title = title;
        _message = message;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);

            ImGuiSC.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.TextWrapped(_message);

                // Right-align button
                float pos = ImGui.CalcTextSize("Close"u8).X + ImGui.GetStyle().WindowPadding.X * 2;
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - pos);
                if (ImGui.Button("Close"u8)) { DialogResult = false; IsOpen = false; }
                ImGui.EndPopup();
            }
        }
        else
        {
            OnClosed();
            ImGuiCore.GuiComponents.Remove(this);
        }
    }

    public static void Show(string message, string title)
    {
        MessageWindow window = new(message, title);
        window.Open();
    }
}
