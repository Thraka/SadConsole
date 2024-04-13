using ImGuiNET;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.Windows;

public class NewFile : ImGuiWindow
{
    private int _documentSelectedIndex = 0;
    private string _documentName = "";
    public Document? Document;

    public NewFile()
    {
        Title = "New file";
    }

    public void Show()
    {
        IsOpen = true;

        if (!ImGuiCore.GuiComponents.Contains(this))
            ImGuiCore.GuiComponents.Add(this);
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
                DocumentTypeListControl.DrawListBox("##Document Type", 3, ref _documentSelectedIndex, ref Document);

                if (Document != null)
                {
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
                    if (ImGui.Button("Create"))
                    {
                        Document.Create();
                        DialogResult = true;
                        IsOpen = false;
                    }

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
