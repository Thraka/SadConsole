using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;
using SadConsole.Editor.Windows;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts
{
    public class DebuggingTools: ImGuiObjectBase
    {
        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (ImGuiCore.GuiTopBar.ShowDemoWindow)
                ImGui.ShowDemoWindow();
            if (ImGuiCore.GuiTopBar.ShowMetrics)
                ImGui.ShowMetricsWindow();
        }
    }

    public class GuiTopBar : ImGuiObjectBase
    {
        public bool ShowDemoWindow;
        public bool ShowMetrics;
        private bool _debug;

        public List<(Vector4 Color, string Text)> StatusItems = new();

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
                    if (ImGui.MenuItem("Open", "o"))
                    {
                        OpenFile window = new();
                        window.Closed += (s, e) =>
                        {
                            if (window.DialogResult)
                            {
                                ImGuiCore.State.OpenDocuments = [.. ImGuiCore.State.OpenDocuments, window.Document!];
                                ImGuiCore.State.SelectedDocumentIndex = ImGuiCore.State.OpenDocuments.Length - 1;
                            }
                        };
                        window.Show();
                    }
                    if (ImGuiCore.State.SelectedDocumentIndex != -1)
                    {
                        ImGui.Separator();
                        if (ImGui.MenuItem("Save", "s"))
                        {
                            SaveFile window = new();
                            window.Show(ImGuiCore.State.GetOpenDocument());
                        }
                        if (ImGui.MenuItem("Close", "c"))
                        {

                        }
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Options"))
                {
                    //if (ImGui.MenuItem("Show SadConsole game", "s", ref GuiState.ShowSadConsoleRendering, true))
                    //    GuiState.RaiseShowSadConsoleRenderingChanged();
                    
                    ImGui.MenuItem("Show Demo", "s", ref ShowDemoWindow);
                    ImGui.MenuItem("Show Metrics", "d", ref ShowMetrics);
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

                // Write status items at the top
                foreach ((Vector4 Color, string Text) item in StatusItems)
                {
                    if (item.Color == Vector4.Zero)
                        ImGui.Text(item.Text);
                    else
                        ImGui.TextColored(item.Color, item.Text);
                }
                StatusItems.Clear();
                ImGui.EndMainMenuBar();
            }
        }
    }
}
