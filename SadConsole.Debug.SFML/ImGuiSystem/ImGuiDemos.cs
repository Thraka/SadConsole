using System.Numerics;
using Hexa.NET.ImGui;

namespace SadConsole.ImGuiSystem;

public class ImGuiDemos : ImGuiObjectBase
{
    private bool show_test_window = false;
    private bool show_metrics_window = false;
    private bool _pauseForEdit;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (_pauseForEdit)
        {
            _pauseForEdit = false;
            System.Diagnostics.Debugger.Break();
        }

        ImGui.Separator();

        if (ImGui.Button("ImGui samples window")) show_test_window = !show_test_window;
        if (ImGui.Button("ImGui Metrics")) show_metrics_window = !show_metrics_window;
        if (ImGui.Button("Break and edit window")) _pauseForEdit = true;

        // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
        if (show_test_window)
        {
            ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
            ImGui.ShowDemoWindow(ref show_test_window);
        }

        if (show_metrics_window)
            ImGui.ShowMetricsWindow(ref show_metrics_window);
    }
}
