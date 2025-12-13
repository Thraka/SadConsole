using System.Numerics;
using Hexa.NET.ImGui;
using SadConsole.ImGuiSystem;

namespace SadConsole.Editor.GuiObjects;

public class GuiDockSpace : ImGuiObjectBase
{
    public const string ID_LEFT_PANEL = "Documents##LeftPanel";
    public const string ID_TOP_PANEL = "TopPanel";
    public const string ID_RIGHT_PANEL = "Tooling##RightPanel";
    public const string ID_BOTTOM_PANEL = "BottomPanel";
    public const string ID_CENTER_PANEL = "Editing##CenterPanel";

    public static ImGuiWindowClass NoTabBarDock;
    public static ImGuiWindowClass NoDockingOverMe;

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

        bool isFirstRun = false;

        if (ImGuiP.DockBuilderGetNode(idRootDockspace) == ImGuiDockNodePtr.Null)
            isFirstRun = true;

        ImGui.DockSpace(idRootDockspace, new Vector2(0.0f, 0.0f), dockspace_flags);
        ImGui.End();

        if (!_runOnce)
        {
            _runOnce = true;

            NoTabBarDock = new ImGuiWindowClass { DockNodeFlagsOverrideSet = (ImGuiDockNodeFlags)ImGuiDockNodeFlagsPrivate.NoTabBar };
            NoDockingOverMe = new ImGuiWindowClass
            {
                DockNodeFlagsOverrideSet =// (ImGuiDockNodeFlags)ImGuiDockNodeFlags.NoDockingSplit
                                           (ImGuiDockNodeFlags)ImGuiDockNodeFlagsPrivate.NoDockingOverMe
            };
            
            Vector2 workCenter = ImGui.GetMainViewport().GetWorkCenter();

            //ImGuiP.DockBuilderRemoveNode(idRootDockspace);             // Clear any preexisting layouts associated with this ID
            ImGuiP.DockBuilderAddNode(idRootDockspace, ImGuiDockNodeFlags.PassthruCentralNode);                // Create a new dock node to use

            float heightSpace = ImGui.GetFrameHeight();

            Vector2 size = new(ImGui.GetMainViewport().WorkSize.X, ImGui.GetMainViewport().WorkSize.Y - heightSpace);
            Vector2 nodePos = new(workCenter.X - size.X * 0.5f, workCenter.Y - size.Y * 0.5f + heightSpace / 2);

            ImGuiP.DockBuilderSetNodeSize(idRootDockspace, size);
            ImGuiP.DockBuilderSetNodePos(idRootDockspace, nodePos);

            //if (isFirstRun)
            {
                uint idLeftPanel = 0;
                uint idTopPanel = 0;
                uint idCenterPanel = 0;
                uint idRightPanel = 0;
                uint idBottomPanel = 0;

                ImGuiP.DockBuilderSplitNode(idRootDockspace, ImGuiDir.Left, 0.2f, ref idLeftPanel, ref idCenterPanel);
                ImGuiP.DockBuilderSplitNode(idCenterPanel, ImGuiDir.Right, 0.3f, ref idRightPanel, ref idCenterPanel);
                ImGuiP.DockBuilderSplitNode(idCenterPanel, ImGuiDir.Up, 0.3f, ref idTopPanel, ref idCenterPanel);
                ImGuiP.DockBuilderSplitNode(idCenterPanel, ImGuiDir.Down, 0.3f, ref idBottomPanel, ref idCenterPanel);

                ImGuiP.DockBuilderDockWindow(ID_LEFT_PANEL, idLeftPanel);
                ImGuiP.DockBuilderDockWindow(ID_TOP_PANEL, idTopPanel);
                ImGuiP.DockBuilderDockWindow(ID_RIGHT_PANEL, idRightPanel);
                ImGuiP.DockBuilderDockWindow(ID_BOTTOM_PANEL, idBottomPanel);
                ImGuiP.DockBuilderDockWindow(ID_CENTER_PANEL, idCenterPanel);
            }

            ImGuiP.DockBuilderFinish(idRootDockspace);
        }
    }
}
