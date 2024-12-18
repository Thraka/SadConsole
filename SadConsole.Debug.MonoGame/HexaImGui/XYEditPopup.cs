﻿using System.Numerics;
using SadConsole.ImGuiSystem;

namespace Hexa.NET.ImGui;

public static partial class ImGui2
{
    public static bool XYEditPopup(string id, ref int xValue, ref int yValue, string xText, string yText, int inputWidth = 100)
    {
        bool returnValue = false;
        //Vector2 mousePosition = ImGui.GetMousePos();
        //ImGui.SetNextWindowPos(mousePosition + new Vector2(10f, 10f));

        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, 4f);
        if (ImGui.BeginPopup(id))
        {
            ImGui.PopStyleVar();

            ImGui.BeginTable("popuptable", 2, ImGuiTableFlags.NoBordersInBody);
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(xText);
            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(inputWidth);
            ImGui.InputInt("##x", ref xValue);


            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(yText);
            ImGui.TableSetColumnIndex(1);
            ImGui.SetNextItemWidth(inputWidth);
            ImGui.InputInt("##y", ref yValue);

            ImGui.EndTable();
            ImGui.Separator();

            if (ImGui.Button("Cancel", new Vector2(50, 0)))
                ImGui.CloseCurrentPopup();

            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X - 50);

            if (ImGui.Button("Accept", new Vector2(50, 0)))
            {
                returnValue = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
        else
        {
            ImGui.PopStyleVar();
        }

        return returnValue;
    }

}
