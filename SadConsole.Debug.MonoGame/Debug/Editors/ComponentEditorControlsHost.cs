using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Components;
using SadConsole.ImGuiSystem;
using SadConsole.UI;

namespace SadConsole.Debug.Editors;

internal class ComponentEditorControlHost : ISadComponentPanel
{
    private ControlHost _stateComponent;
    private IScreenObject _screenObject;

    private int _currentLayer = 0;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state, IComponent component)
    {
        // Capture the state information of the component as it is
        if (_screenObject != state.Object ||
            _stateComponent != state.Object.SadComponents[state.ComponentsSelectedItem])
        {
            _screenObject = state.Object;
            _stateComponent = (ControlHost)state.Object.SadComponents[state.ComponentsSelectedItem];
        }

        // Draw the ImGUI interface
        ImGui.BeginGroup();
        {
            ImGui.Text($"Controls: {_stateComponent.Count}");

        }
        ImGui.EndGroup();

        //ImGui.Begin(GuiDockspace.ID_CENTER_PANEL, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
        //{
        //    if (ImGui.BeginTabBar("preview_tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
        //    {
        //        if (ImGui.BeginTabItem("Another tab item", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
        //        {
        //            ImGui.Text($"Layer: {_stateComponent.Count}");
        //            ImGui.EndTabItem();
        //        }

        //        ImGui.EndTabBar();
        //    }
        //}
        //ImGui.End();
    }
}
