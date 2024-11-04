using System.Numerics;
using ImGuiNET;
using SadConsole.Editor.Model;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiParts;

public class WindowTools : ImGuiObjectBase
{
    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGui.Begin("Tools");

        if (ImGuiCore.State.SelectedDocumentIndex != -1)
        {
            var doc = ImGuiCore.State.GetOpenDocument();

            // If the document supports the tools window
            if (doc is IDocumentTools docTools)
            {
                // Display the default tools list
                if (docTools.ShowToolsList)
                {
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    if (docTools.State.ToolObjects.Length != 0)
                    {
                        int selectedToolIndex = docTools.State.SelectedToolIndex;

                        ImGui.TextDisabled("(?)");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 25.0f);
                            ImGui.TextUnformatted(docTools.State.SelectedTool.Description);
                            ImGui.PopTextWrapPos();
                            ImGui.EndTooltip();
                        }
                        ImGui.SameLine();

                        ImGui.SetNextItemWidth(ImGui.GetContentRegionMax().X - ImGui.GetCursorPosX());
                        if (ImGui.Combo("##toolsList", ref selectedToolIndex, docTools.State.ToolNames, docTools.State.ToolObjects.Length, docTools.State.ToolObjects.Length <= 4 ? 4 : 6))
                        {
                            if (docTools.State.SelectedToolIndex != selectedToolIndex)
                            {
                                Editor.Tools.ITool oldTool = docTools.State.SelectedTool!;

                                docTools.State.SelectedTool.OnDeselected();
                                docTools.State.SelectedToolIndex = selectedToolIndex;
                                docTools.ToolChanged(oldTool, docTools.State.SelectedTool);
                                docTools.State.SelectedTool.OnSelected();
                            }
                        }

                        docTools.State.SelectedTool.BuildSettingsPanel(renderer);
                    }
                    else
                    {
                        docTools.State.SelectedToolIndex = -1;
                        ImGui.TextColored(Color.MediumVioletRed.ToVector4(), "No tools associated with this document");
                    }
                }

                // Build any custom rendering afterwards
                docTools.BuildUI(renderer);
            }
        }
        else
        {
            ImGui.TextColored(Color.MediumVioletRed.ToVector4(), "No document selected");
        }


        ImGui.End();
    }
}
