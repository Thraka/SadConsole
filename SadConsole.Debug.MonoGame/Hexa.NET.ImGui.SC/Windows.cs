namespace Hexa.NET.ImGui.SC;

public static partial class ImGuiSC
{
    public static void CenterNextWindow() =>
        ImGui.SetNextWindowPos(ImGui.GetIO().DisplaySize / 2f, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));
}
