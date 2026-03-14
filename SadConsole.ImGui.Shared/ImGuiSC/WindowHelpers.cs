namespace Hexa.NET.ImGui;

public static partial class ImGuiSC
{
    public static bool WindowDrawButtons(out bool result, bool acceptDisabled = false, string cancelButtonText = "Cancel", string acceptButtonText = "Accept")
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

    public static void CenterNextWindow() =>
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2f, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));
}
