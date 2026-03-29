using System.Numerics;

namespace Hexa.NET.ImGui;

public static partial class ImGuiSC
{
    public static bool WindowDrawButtons(out bool result, bool acceptDisabled = false, string cancelButtonText = "Cancel", string acceptButtonText = "Accept")
    {
        bool buttonClicked = false;
        result = false;

        // Cancel Button (show if not empty/null)
        if (!string.IsNullOrEmpty(cancelButtonText) && ImGui.Button(cancelButtonText))
            buttonClicked = true;

        // Accept Button -- Right-aligned (show if not empty/null)
        if (!string.IsNullOrEmpty(acceptButtonText))
        {
            float pos = ImGui.CalcTextSize(acceptButtonText).X + ImGui.GetStyle().ItemSpacing.X + ImGui.GetStyle().FramePadding.X * 2 + 2;
            if (!string.IsNullOrEmpty(cancelButtonText))
                ImGui.SameLine(ImGui.GetWindowWidth() - pos);
            else
                ImGui.SetCursorPosX(ImGui.GetWindowWidth() - pos);

            ImGui.BeginDisabled(acceptDisabled);

            if (ImGui.Button(acceptButtonText))
            {
                buttonClicked = true;
                result = true;
            }
            ImGui.EndDisabled();
        }

        return buttonClicked;
    }

    public static void CenterNextWindow() =>
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2f, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));
    public static void CenterNextWindowOnAppearing(Vector2 size)
    {
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2f, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));
        ImGui.SetNextWindowSize(size, ImGuiCond.Appearing);
    }
}
