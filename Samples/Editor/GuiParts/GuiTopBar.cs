using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts
{
    public class GuiTopBar : ImGuiObjectBase
    {
        private bool _showDemoWindow;
        private bool _debug;

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (_debug)
            {
                _debug = false;
                System.Diagnostics.Debugger.Break();
            }

            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New", "n"))
                    {
                        ImGuiCore.ShowCreateDocument();
                    }
                    if (ImGui.MenuItem("Close", "c"))
                    {
                        
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Options"))
                {
                    //if (ImGui.MenuItem("Show SadConsole game", "s", ref GuiState.ShowSadConsoleRendering, true))
                    //    GuiState.RaiseShowSadConsoleRenderingChanged();
                    
                    ImGui.MenuItem("Show demo", "s", ref _showDemoWindow);

                    //ImGui.MenuItem("Show surface preview", "p", ref GuiState.ShowSurfacePreview, true);
                    //ImGui.MenuItem("Show final output", "o", ref GuiState.GuiFinalOutputWindow.IsOpen, true);
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Debug"))
                {
                    if (ImGui.MenuItem("Pause", "p"))
                        _debug = true;

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }

            if (_showDemoWindow)
                ImGui.ShowDemoWindow();
        }
    }
}
