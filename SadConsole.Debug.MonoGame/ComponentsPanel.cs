using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

public class ComponentsPanel: ImGuiObjectBase
{
    public static Dictionary<Type, ImGuiObjectBase> RegisteredPanels = new Dictionary<Type, ImGuiObjectBase>();

    public ScreenObjectState CurrentScreenObject;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state)
    {
        CurrentScreenObject = state;
        BuildUI(renderer);
        CurrentScreenObject = null;
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (CurrentScreenObject == null) return;
        if (CurrentScreenObject.Object.SadComponents.Count == 0)
        {
            ImGui.Text("No components on object");
            return;
        }

        //ImGui.BeginChild(id, new Vector2(0, 300), false, ImGuiWindowFlags.HorizontalScrollbar);
        ImGui.BeginChild("screen_object_components", new Vector2(0, 0), ImGuiWindowFlags.MenuBar);
        {
            if (ImGui.BeginMenuBar())
            {
                ImGui.Text("Components");
                ImGui.EndMenuBar();
            }

            ///////
            // Components
            ///////
            if (CurrentScreenObject.Object.SadComponents.Count != 0)
            {
                ImGui.ListBox("##Components", ref CurrentScreenObject.ComponentsSelectedItem, CurrentScreenObject.Components, CurrentScreenObject.Object.SadComponents.Count, 3);

                ImGui.Text($"{CurrentScreenObject.Components[CurrentScreenObject.ComponentsSelectedItem]} Settings");

                ImGui.Separator();

                ///////
                // Custom editors
                ///////
                if (RegisteredPanels.TryGetValue(CurrentScreenObject.Object.SadComponents[CurrentScreenObject.ComponentsSelectedItem].GetType(), out var panel))
                    panel.BuildUI(renderer);
                else
                    ImGui.Text("None");
            }
        }
        ImGui.EndChild();
    }
}
