using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ImGuiNET;
using SadConsole.ImGuiSystem;
using ImGuiInternal = ImGuiNET.Internal.ImGui;

namespace SadConsole.Editor.GuiParts;

public class GuiDockspace : ImGuiObjectBase
{
    private bool p_open;
    private bool runOnce = false;

    public unsafe override void BuildUI(ImGuiRenderer renderer)
    {
        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.Pos);
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.SetNextWindowBgAlpha(0.0f);

        ImGuiWindowFlags window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;
        window_flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        window_flags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
        ImGui.Begin("RootDockspaceMain", ref p_open, window_flags);
        ImGui.PopStyleVar(3);

        var id = ImGui.GetID("RootDockspace");
        ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.PassthruCentralNode;
        ImGui.DockSpace(id, new Vector2(0.0f, 0.0f), dockspace_flags);
        ImGui.End();

        if (!runOnce)
        {
            runOnce = true;

            var workCenter = ImGui.GetMainViewport().GetWorkCenter();

            ImGuiInternal.DockBuilderRemoveNode(id);             // Clear any preexisting layouts associated with the ID we just chose
            ImGuiInternal.DockBuilderAddNode(id, ImGuiDockNodeFlags.PassthruCentralNode);                // Create a new dock node to use

            var heightSpace = ImGui.GetFrameHeight();

            var size = new Vector2(ImGui.GetMainViewport().WorkSize.X, ImGui.GetMainViewport().WorkSize.Y - heightSpace);
            var nodePos = new Vector2(workCenter.X - size.X * 0.5f, workCenter.Y - size.Y * 0.5f + (heightSpace / 2));

            ImGuiInternal.DockBuilderSetNodeSize(id, size);
            ImGuiInternal.DockBuilderSetNodePos(id, nodePos);

            uint singleDocSide;

            ImGuiInternal.DockBuilderSplitNode(id, ImGuiDir.Up, 0.5f, out uint documentsSide, out uint toolsSide);
            singleDocSide = ImGuiInternal.DockBuilderSplitNode(id, ImGuiDir.Right, 0.7f, out _, out _);
            //var dock3 = ImGuiInternal.DockBuilderSplitNode(id, ImGuiDir.Right, 0.5f, out _, out _);

            ImGuiInternal.DockBuilderDockWindow("Active Documents", documentsSide);
            ImGuiInternal.DockBuilderDockWindow("Tools", toolsSide);
            ImGuiInternal.DockBuilderDockWindow("Open Document", singleDocSide);

            ImGuiInternal.DockBuilderFinish(id);
        }

        //ImGui.Begin("Tools");
        //ImGui.End();

        //ImGui.Begin("Active Documents");
        //ImGui.End();
        
        //ImGui.Begin("Open Document");
        //ImGui.End();
    }
}
