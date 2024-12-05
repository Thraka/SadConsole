using System.Linq;
using System.Numerics;
using SadConsole.ImGuiSystem;
using Hexa.NET.ImGui;

namespace SadConsole.Debug;

class ScreenObjectsPanel : ImGuiObjectBase
{
    private float f = 0.0f;
    private bool _toggle_screenObj_doDraw;
    private bool show_test_window = false;
    private bool show_metrics_window = false;
    private bool show_another_window = false;
    private Vector3 clear_color = new Vector3(114f / 255f, 144f / 255f, 154f / 255f);
    private byte[] _textBuffer = new byte[100];
    private bool _pauseForEdit;

    private ImGuiDemos _guiHelpers = new();
    private ScreenObjectDetailsPanel _guiDetails = new();
    private ComponentsPanel _guiComponents = new();

    public override void BuildUI(ImGuiRenderer renderer)
    {
        if (_pauseForEdit)
        {
            _pauseForEdit = false;
            System.Diagnostics.Debugger.Break();
        }

        ImGui.SetNextWindowClass(ref GuiDockspace.NoTabBarDock);

        ImGui.Begin(GuiDockspace.ID_LEFT_PANEL);
        {
            // Screen objects list
            ImGui.SeparatorText("Current Scene");

            // TODO: Tighten this logic up
            foreach (ScreenObjectState item in GuiState.ScreenObjectUniques.Values)
                item.Found = false;

            if (ImGui.BeginListBox("##screen_objs_list", new Vector2(-1, 200)))
            {
                CreateScreenObject(GameHost.Instance.Screen);
                ImGui.EndListBox();
            }

            foreach (ScreenObjectState item in GuiState.ScreenObjectUniques.Values.ToArray())
                if (!item.Found)
                    GuiState.ScreenObjectUniques.Remove(item.Object);

            if (GuiState._selectedScreenObject != null && !GuiState.ScreenObjectUniques.ContainsKey(GuiState._selectedScreenObject))
            {
                GuiState._selectedScreenObject = null;
                GuiState._selectedScreenObjectState = null;
            }

            if (GuiState._selectedScreenObjectState != null)
            {
                ImGui.SeparatorText(GuiState._selectedScreenObjectState.Object.ToString());
                _guiDetails.BuildUI(renderer, GuiState._selectedScreenObjectState);
                _guiComponents.BuildUI(renderer, GuiState._selectedScreenObjectState);
            }

            ImGui.Separator();
        }

        //_guiHelpers.BuildUI(renderer);


        ImGui.End();
    }

    void CreateScreenObject(IScreenObject obj)
    {
        const ImGuiTreeNodeFlags baseTreeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick
                                                 | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.DefaultOpen;

        ImGuiTreeNodeFlags treeFlags = baseTreeFlags;

        ScreenObjectState state;

        if (GuiState.ScreenObjectUniques.TryGetValue(obj, out ScreenObjectState value))
        {
            state = value;
            state.Found = true;
        }
        else
        {
            state = ScreenObjectState.Create(obj);
            GuiState.ScreenObjectUniques[obj] = state;
        }

        if (GuiState._selectedScreenObject == obj)
            treeFlags |= ImGuiTreeNodeFlags.Selected;

        if (obj.Children.Count == 0)
        {
            treeFlags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

            if (obj.IsFocused) ImGui.PushStyleColor(ImGuiCol.Text, Debugger.Settings.Color_FocusedObj);

            ImGui.TreeNodeEx(state.Identifier.ToString(), treeFlags, obj.ToString());

            if (obj.IsFocused) ImGui.PopStyleColor();

            //if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
            //{
            //    ImGui.SetDragDropPayload("SCREEN_OBJ", new IntPtr(state.Identifier), sizeof(int));
            //    ImGui.EndDragDropSource();
            //}
            //else if (ImGui.BeginDragDropTarget())
            //    ImGui.AcceptDragDropPayload("SCREEN_OBJ");
            //else
            if (ImGui.IsItemClicked())
                SetScreenObject(obj, state);
        }
        else
        {
            if (obj.IsFocused) ImGui.PushStyleColor(ImGuiCol.Text, Debugger.Settings.Color_FocusedObj);

            bool opened = ImGui.TreeNodeEx(state.Identifier.ToString(), treeFlags, obj.ToString());

            if (obj.IsFocused) ImGui.PopStyleColor();

            if (ImGui.IsItemClicked())
                SetScreenObject(obj, state);

            if (opened)
            {
                foreach (IScreenObject item in obj.Children)
                    CreateScreenObject(item);

                ImGui.TreePop();
            }
        }
    }

    private void SetScreenObject(IScreenObject obj, ScreenObjectState state)
    {
        GuiState._selectedScreenObject = obj;
        GuiState._selectedScreenObjectState = state;
        _toggle_screenObj_doDraw = obj.IsVisible;
    }



}
