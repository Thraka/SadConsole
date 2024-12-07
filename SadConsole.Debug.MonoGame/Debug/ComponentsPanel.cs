using System;
using System.Collections.Generic;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

public class ComponentsPanel: ImGuiObjectBase
{
    public static Dictionary<Type, Editors.ISadComponentPanel> RegisteredPanels { get; } = [];

    public ScreenObjectState CurrentScreenObject;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state)
    {
        CurrentScreenObject = state;
        BuildUI(renderer);
        CurrentScreenObject = null;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui2.SeparatorText("Components", Debugger.Settings.Color_PanelHeader);
        ImGui.Separator();

        if (CurrentScreenObject == null) return;
        if (CurrentScreenObject.Object.SadComponents.Count == 0)
        {
            ImGui.Text("No components on object");
            return;
        }

        ///////
        // Components
        ///////
        if (CurrentScreenObject.Object.SadComponents.Count != 0)
        {
            ImGui.SetNextItemWidth(-1f);
            ImGui.ListBox("##Components", ref CurrentScreenObject.ComponentsSelectedItem,
                          CurrentScreenObject.Components, CurrentScreenObject.Object.SadComponents.Count, 3);

            ImGui2.SeparatorText($"{CurrentScreenObject.Components[CurrentScreenObject.ComponentsSelectedItem]} Settings", Debugger.Settings.Color_PanelHeader);

            ImGui.Separator();

            ///////
            // Custom editors
            ///////
            if (RegisteredPanels.TryGetValue(CurrentScreenObject.Object.SadComponents[CurrentScreenObject.ComponentsSelectedItem].GetType(),
                    out var panel))

                panel.BuildUI(renderer, CurrentScreenObject, CurrentScreenObject.Object.SadComponents[CurrentScreenObject.ComponentsSelectedItem]);

            else
                ImGui.Text("No editor associated with this component");
        }
    }
}
