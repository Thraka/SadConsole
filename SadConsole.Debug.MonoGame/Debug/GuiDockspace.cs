using System.Numerics;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

public class GuiDockspace : ImGuiObjectBase
{
    public const string ID_LEFT_PANEL = "Scene##LeftPanel";
    public const string ID_CENTER_PANEL = "Previews##RightPanel";
    public const string ID_RIGHT_PANEL = "Extras##CenterPanel";

    public static ImGuiWindowClass NoTabBarDock;
    public static ImGuiWindowClass AutoHideTabBar;

    private bool p_open;
    private bool _runOnce = false;

    public override void BuildUI(ImGuiRenderer renderer)
    {
        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.Pos);
        ImGui.SetNextWindowSize(viewport.Size);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.SetNextWindowBgAlpha(0.0f);

        ImGuiWindowFlags window_flags = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking
                                      | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse
                                      | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove
                                      | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));
        ImGui.Begin("DockSpace Demo", ref p_open, window_flags);
        ImGui.PopStyleVar(3);

        uint idRootDockspace = ImGui.GetID("RootDockspace");
        ImGuiDockNodeFlags dockspace_flags = ImGuiDockNodeFlags.PassthruCentralNode;
        ImGui.DockSpace(idRootDockspace, new Vector2(0.0f, 0.0f), dockspace_flags);
        ImGui.End();

        if (!_runOnce)
        {
            _runOnce = true;

            NoTabBarDock = new() { DockNodeFlagsOverrideSet = (ImGuiDockNodeFlags)ImGuiDockNodeFlagsPrivate.NoTabBar };
            AutoHideTabBar = new() { DockNodeFlagsOverrideSet = ImGuiDockNodeFlags.AutoHideTabBar };
            Vector2 workCenter = ImGui.GetMainViewport().GetWorkCenter();

            ImGuiP.DockBuilderRemoveNode(idRootDockspace);             // Clear any preexisting layouts associated with the ID we just chose
            ImGuiP.DockBuilderAddNode(idRootDockspace, ImGuiDockNodeFlags.PassthruCentralNode);                // Create a new dock node to use

            float heightSpace = ImGui.GetFrameHeight();

            Vector2 size = new(ImGui.GetMainViewport().WorkSize.X, ImGui.GetMainViewport().WorkSize.Y - heightSpace);
            Vector2 nodePos = new(workCenter.X - size.X * 0.5f, workCenter.Y - size.Y * 0.5f + heightSpace / 2);

            ImGuiP.DockBuilderSetNodeSize(idRootDockspace, size);
            ImGuiP.DockBuilderSetNodePos(idRootDockspace, nodePos);

            uint idLeftPanel = 0;
            uint idRightPanel = 0;
            uint idCenterPanel = 0;

            ImGuiP.DockBuilderSplitNode(idRootDockspace, ImGuiDir.Left, 0.3f, ref idLeftPanel, ref idCenterPanel);
            ImGuiP.DockBuilderSplitNode(idCenterPanel, ImGuiDir.Right, 0.3f, ref idRightPanel, ref idCenterPanel);

            ImGuiP.DockBuilderDockWindow(ID_LEFT_PANEL, idLeftPanel);
            ImGuiP.DockBuilderDockWindow(ID_RIGHT_PANEL, idRightPanel);
            ImGuiP.DockBuilderDockWindow(ID_CENTER_PANEL, idCenterPanel);

            ImGuiP.DockBuilderFinish(idRootDockspace);
        }
    }
}
