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

        ImGui.Begin("Screen objects");
        {
            // Screen objects list
            ImGui.BeginChild("screen_objects", new Vector2(200, 300), ImGuiWindowFlags.MenuBar);
            {
                if (ImGui.BeginMenuBar())
                {
                    ImGui.Text("Screen objects");
                    ImGui.EndMenuBar();
                }

                // TODO: Tighten this logic up
                foreach (ScreenObjectState item in GuiState.ScreenObjectUniques.Values)
                    item.Found = false;

                CreateScreenObject(GameHost.Instance.Screen);

                foreach (ScreenObjectState item in GuiState.ScreenObjectUniques.Values.ToArray())
                    if (!item.Found)
                        GuiState.ScreenObjectUniques.Remove(item.Object);

                if (GuiState._selectedScreenObject != null && !GuiState.ScreenObjectUniques.ContainsKey(GuiState._selectedScreenObject))
                {
                    GuiState._selectedScreenObject = null;
                    GuiState._selectedScreenObjectState = null;
                }
            }
            ImGui.EndChild();

            // Show right-side properties
            if (GuiState._selectedScreenObjectState != null)
            {
                ImGui.SameLine();
                _guiDetails.BuildUI(renderer, GuiState._selectedScreenObjectState);
                _guiComponents.BuildUI(renderer, GuiState._selectedScreenObjectState);
            }

            ImGui.Separator();

            //ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
        }

        _guiHelpers.BuildUI(renderer);


        ImGui.End();
    }

    void CreateScreenObject(IScreenObject obj)
    {
        const ImGuiTreeNodeFlags baseTreeFlags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.SpanAvailWidth;

        ImGuiTreeNodeFlags treeFlags = baseTreeFlags;

        ScreenObjectState state;

        if (GuiState.ScreenObjectUniques.ContainsKey(obj))
        {
            state = GuiState.ScreenObjectUniques[obj];
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
            ImGui.TreeNodeEx(state.Identifier.ToString(), treeFlags, obj.ToString());

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
            bool opened = ImGui.TreeNodeEx(state.Identifier.ToString(), treeFlags, obj.ToString());

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
