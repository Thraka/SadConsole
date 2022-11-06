using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using System.Numerics;
using SadRogue.Primitives;
using SadConsole.ImGuiSystem;

namespace SadConsole.Debug.MonoGame
{
    class GuiScreenObjects : ImGuiObjectBase
    {
        private float f = 0.0f;
        private bool _toggle_screenObj_doDraw;
        private bool show_test_window = false;
        private bool show_metrics_window = false;
        private bool show_another_window = false;
        private Vector3 clear_color = new Vector3(114f / 255f, 144f / 255f, 154f / 255f);
        private byte[] _textBuffer = new byte[100];
        private bool _pauseForEdit;

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
                ImGui.BeginChild("screen_objects", new Vector2(200, 300), true, ImGuiWindowFlags.MenuBar);
                {
                    if (ImGui.BeginMenuBar())
                    {
                        ImGui.Text("Screen objects");
                        ImGui.EndMenuBar();
                    }

                    //ImGui.LabelText("Items", "");
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
                    ImGuiObjects.ScreenObjectPanel.Begin("selected_object_panel", GuiState._selectedScreenObjectState);
                    ImGuiObjects.ScreenObjectPanel.End();
                }

                ImGui.Separator();

                if (GuiState._selectedScreenObject != null)
                {
                    if (ImGui.Checkbox("Do draw", ref _toggle_screenObj_doDraw))
                    {
                        GuiState._selectedScreenObject.IsVisible = _toggle_screenObj_doDraw;
                    }
                    if (ImGui.Button("Fill with random garbage"))
                    {
                        if (GuiState._selectedScreenObject is ScreenSurface surface) surface.Surface.FillWithRandomGarbage(surface.Font);
                    }
                }

                ImGui.Separator();

                if (ImGui.Button("ImGui samples window")) show_test_window = !show_test_window;
                if (ImGui.Button("ImGui Metrics")) show_metrics_window = !show_metrics_window;
                if (ImGui.Button("Break and edit window")) _pauseForEdit = true;

                //ImGui.Image(_imGuiTexture, new Num.Vector2(300, 150), Num.Vector2.Zero, Num.Vector2.One, Num.Vector4.One, Num.Vector4.One); // Here, the previously loaded texture is used
            }

            // 2. Show another simple window, this time using an explicit Begin/End pair
            if (show_another_window)
            {
                ImGui.SetNextWindowSize(new Vector2(200, 100), ImGuiCond.FirstUseEver);
                ImGui.Begin("Another Window", ref show_another_window);
                ImGui.Text("Hello");
                ImGui.End();
            }

            // 3. Show the ImGui test window. Most of the sample code is in ImGui.ShowTestWindow()
            if (show_test_window)
            {
                ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref show_test_window);
            }

            if (show_metrics_window)
            {
                ImGui.ShowMetricsWindow(ref show_metrics_window);
            }
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

                if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
                {
                    ImGui.SetDragDropPayload("SCREEN_OBJ", new IntPtr(state.Identifier), sizeof(int));
                    ImGui.EndDragDropSource();
                }
                else if (ImGui.BeginDragDropTarget())
                {
                    var stuff = ImGui.AcceptDragDropPayload("SCREEN_OBJ");
                }
                else if (ImGui.IsItemClicked())
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
}
