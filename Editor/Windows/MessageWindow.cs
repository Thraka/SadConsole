using Hexa.NET.ImGui;
using SadConsole.Editor;
using SadConsole.ImGuiSystem;
using SadConsole.ImGuiSystem.Rendering;

namespace SadConsole.ImGuiSystem;

public class MessageWindow: ImGuiObjectBase
{
    private string _message;
    private string _title;
    private string _id;
    private bool _firstShow = true;

    private MessageWindow(string id, string message, string title = "Message")
    {
        _id = id;
        _title = title;
        _message = message;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (_firstShow)
        {
            ImGui.OpenPopup(_id);
            _firstShow = false;
        }

        ImGuiSC.CenterNextWindowOnAppearing(new System.Numerics.Vector2(350, -1));
        if (ImGui.BeginPopupModal(_id, ImGuiWindowFlags.NoResize))
        {
            ImGui.TextWrapped(_message);

            // Right-align button
            float pos = ImGui.CalcTextSize("Close"u8).X + ImGui.GetStyle().WindowPadding.X * 2;
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - pos);
            if (ImGui.Button("Close"u8))
            {
                ImGui.CloseCurrentPopup();
                renderer.UIObjects.Remove(this);
            }
            ImGui.EndPopup();
        }
    }

    public static void Show(ImGuiRenderer renderer, string message, string title)
    {
        MessageWindow window = new(Guid.NewGuid().ToString(), message, title);
        ImGui.OpenPopup(window._id);
        renderer.UIObjects.Add(window);
    }
}
