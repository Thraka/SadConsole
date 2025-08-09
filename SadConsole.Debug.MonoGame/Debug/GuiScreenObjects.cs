using System.Linq;
using System.Numerics;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.SC;

namespace SadConsole.Debug;

public class GuiScreenObjects : ImGuiObjectBase
{
    private float f = 0.0f;
    private bool _toggle_screenObj_doDraw;
    private bool show_test_window = false;
    private bool show_metrics_window = false;
    private bool show_another_window = false;
    private bool _pauseForEdit;

    private ImGuiDemos _guiHelpers = new();
    private ScreenObjectDetailsPanel _guiDetails = new();

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (_pauseForEdit)
        {
            _pauseForEdit = false;
            System.Diagnostics.Debugger.Break();
        }

        ImGui.SetNextWindowClass(ref GuiDockspace.AutoHideTabBar);
        ImGui.Begin(GuiDockspace.ID_LEFT_PANEL);
        {
            // Screen objects list
            ImGuiSC.SeparatorText("Current Scene", Debugger.Settings.Color_PanelHeader);

            // Refresh list of objects
            foreach (ScreenObjectState item in GuiState.ScreenObjectUniques.Values)
                item.Found = false;

            GuiState._hoveredScreenObjectState = null;

            // List the objects
            if (ImGui.BeginListBox("##screen_objs_list", new Vector2(-1, 200)))
            {
                const ImGuiTreeNodeFlags baseTreeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick
                    | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.DefaultOpen;


                ProcessItem(GuiState.ScreenObjectUniques[GameHost.Instance.Screen]);

                void ProcessItem(ScreenObjectState state)
                {
                    ImGuiTreeNodeFlags treeFlags = baseTreeFlags;

                    if (GuiState._selectedScreenObjectState == state)
                        treeFlags |= ImGuiTreeNodeFlags.Selected;

                    if (state.Children.Length == 0)
                        treeFlags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

                    if (state.Object.IsFocused) ImGui.PushStyleColor(ImGuiCol.Text, Debugger.Settings.Color_FocusedObj);
                    bool opened = ImGui.TreeNodeEx(state.Identifier.ToString(), treeFlags, state.ObjectName);
                    if (state.Object.IsFocused) ImGui.PopStyleColor();
                    if (ImGui.IsItemHovered())
                        GuiState._hoveredScreenObjectState = state;

                    //if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
                    //{
                    //    ImGui.SetDragDropPayload("SCREEN_OBJ", new IntPtr(state.Identifier), sizeof(int));
                    //    ImGui.EndDragDropSource();
                    //}
                    //else if (ImGui.BeginDragDropTarget())
                    //    ImGui.AcceptDragDropPayload("SCREEN_OBJ");
                    //else

                    if (ImGui.IsItemClicked())
                    {
                        GuiState._selectedScreenObject = state.Object;
                        GuiState._selectedScreenObjectState = state;
                        _toggle_screenObj_doDraw = state.Object.IsVisible;
                    }

                    if (!treeFlags.HasFlag(ImGuiTreeNodeFlags.Leaf) && opened)
                    {
                        foreach (ScreenObjectState item in state.Children)
                            ProcessItem(item);

                        ImGui.TreePop();
                    }
                }

                ImGui.EndListBox();
            }

            // Object is selected
            if (GuiState._selectedScreenObjectState != null)
            {
                _guiDetails.BuildUI(renderer, GuiState._selectedScreenObjectState);
            }
        }

        //_guiHelpers.BuildUI(renderer);

        ImGui.End();
    }

}
