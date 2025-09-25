using SadConsole.ImGuiSystem;

namespace Hexa.NET.ImGui.SC.Windows;

public class PromptWindow : ImGuiWindowBase
{
    private string _message;
    private string _positiveText;
    private string _negativeText;

    public PromptWindow(string message, string title = "Message", string positiveText = "Yes", string negativeText = "No")
    {
        Title = title;
        _message = message;
        _positiveText = positiveText;
        _negativeText = negativeText;
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

                if(DrawButtons(out DialogResult, false, _negativeText, _positiveText))
                    Close();

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
