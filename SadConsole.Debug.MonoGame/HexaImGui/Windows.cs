namespace Hexa.NET.ImGui;

public static partial class ImGui2
{
    public static void CenterNextWindow() =>
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2f, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));
}
