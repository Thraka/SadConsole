using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts
{
    public class GuiNewFileWindow: ImGuiWindow
    {
        private int _documentSelectedIndex;

        public GuiNewFileWindow()
        {
            Title = "New file";

        }

        public override void BuildUI(ImGuiRenderer renderer)
        {
            if (IsOpen)
            {
                if (ImGui.Begin(Title, ref IsOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
                {
                    ImGui.Text("Document Type:");
                    if (ImGui.ListBox("##Document Type", ref _documentSelectedIndex, new[] { "Surface", "Animation" }, 2))
                    {

                    }

                    ImGui.Text($"id: {_documentSelectedIndex}");



                    if (ImGui.Button("Cancel")) { DialogResult = false; IsOpen = false; }

                    // Right-align button
                    float pos = ImGui.GetItemRectSize().X + ImGui.GetStyle().ItemSpacing.X;
                    ImGui.SameLine(ImGui.GetWindowWidth() - pos);
                    if (ImGui.Button("Create")) { DialogResult = true; IsOpen = false; }
                }
                ImGui.End();
            }
            else
            {
                OnClosed();
                ImGuiCore.GuiComponents.Remove(this);
            }
                
        }
    }
}
