﻿using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.Components;
using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.Editors;

internal class ComponentEditorLayeredSurface : ISadComponentPanel
{
    private LayeredSurface _stateComponent;
    private IScreenObject _screenObject;

    private int _currentLayer = 0;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state, IComponent component)
    {
        // Capture the state information of the component as it is
        if (_screenObject != state.Object ||
            _stateComponent != state.Object.SadComponents[state.ComponentsSelectedItem])
        {
            _screenObject = state.Object;
            _stateComponent = (LayeredSurface)state.Object.SadComponents[state.ComponentsSelectedItem];
        }

        // Draw the ImGUI interface
        ImGui.BeginGroup();
        {
            ImGui.Text($"Layers: {_stateComponent.Count}");

            ImGui.Text($"View Position: {_stateComponent.View.X}, {_stateComponent.View.Y}");
            ImGui.Text($"Width: {_stateComponent.View.Width}");
            ImGui.Text($"Height: {_stateComponent.View.Height}");

            ImGui.Text($"Current Layer: {_currentLayer}");
            ImGui.SameLine();
            ImGui.BeginDisabled(_currentLayer == 0);
            if (ImGui.ArrowButton("downbtn", ImGuiDir.Down))
                _currentLayer--;
            ImGui.EndDisabled();

            ImGui.SameLine();
            ImGui.BeginDisabled(_currentLayer == _stateComponent.Count - 1);
            if (ImGui.ArrowButton("upbtn", ImGuiDir.Up))
                _currentLayer++;
            ImGui.EndDisabled();


        }
        ImGui.EndGroup();

        ImGui.Begin(GuiDockspace.ID_CENTER_PANEL, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
        {
            if (ImGui.BeginTabBar("preview_tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
            {
                if (ImGui.BeginTabItem("Another tab item", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
                {
                    ImGui.Text($"Layer: {_stateComponent.Count}");
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
        ImGui.End();
    }
}
