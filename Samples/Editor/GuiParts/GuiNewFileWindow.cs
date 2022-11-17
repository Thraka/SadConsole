using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using SadConsole.ImGuiSystem;
using SadConsole.UI;

namespace SadConsole.Editor.GuiParts;

public class GuiNewFileWindow: ImGuiWindow
{
    private int _documentSelectedIndex = -1;
    private string _documentName = "";
    public Model.Document Document;

    public GuiNewFileWindow()
    {
        Title = "New file";
    }

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (IsOpen)
        {
            ImGui.OpenPopup(Title);
            
            ImGuiExt.CenterNextWindow();
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, -1));
            if (ImGui.BeginPopupModal(Title, ref IsOpen, ImGuiWindowFlags.NoResize))
            {
                ImGui.Text("Document Type:");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                if (ImGui.ListBox("##Document Type", ref _documentSelectedIndex, new[] { "Surface", "Scene", "Animation" }, 3))
                {
                    Document = _documentSelectedIndex switch
                    {
                        _ => new Model.SurfaceDocument(),
                    };
                }

                if (Document != null)
                {
                    ImGui.Separator();

                    ImGui.Text("Name");
                    ImGui.InputText("##name", ref Document.Name, 50);

                    ImGui.Separator();

                    Document.BuildUINew(renderer);
                }
                ImGui.Separator();

                if (ImGui.Button("Cancel")) { DialogResult = false; IsOpen = false; }

                // Right-align button
                float pos = ImGui.GetItemRectSize().X + ImGui.GetStyle().ItemSpacing.X;
                ImGui.SameLine(ImGui.GetWindowWidth() - pos);

                if (Document == null)
                {
                    ImGui.BeginDisabled();
                    ImGui.Button("Create");
                    ImGui.EndDisabled();
                }
                else
                    if (ImGui.Button("Create")) { DialogResult = true; IsOpen = false; }

                ImGui.EndPopup();
            }
        }
        else
        {
            OnClosed();
            ImGuiCore.GuiComponents.Remove(this);
        }
            
    }
}
