using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using System.Numerics;
using SadConsole.Numerics;
using SadRogue.Primitives;

namespace SadConsole.Debug.MonoGame
{
    class GuiScreenObjects : ImGuiObjectBase
    {
        public GuiScreenObjects()
        {
        }

        private float f = 0.0f;
        private bool _toggle_screenObj_doDraw;
        private bool show_test_window = false;
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
            
            // 1.Show a simple window
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

                if (GuiState._selectedScreenObjectState != null)
                {
                    ImGui.SameLine();
                    ImGui.BeginChild("selected_object_pane", new Vector2(0, 300), false);
                    {
                        ///////
                        // Position X
                        ///////
                        ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Position X: ");
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(100);
                        if (ImGui.InputInt("##x", ref GuiState._selectedScreenObjectState.PositionX))
                        {
                            GuiState._selectedScreenObject.Position = GuiState._selectedScreenObject.Position.WithX(GuiState._selectedScreenObjectState.PositionX);
                            GuiState._selectedScreenObjectState.Refresh();
                        }

                        ///////
                        // Width
                        ///////
                        ImGui.SameLine();
                        ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Width: ");
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(80);
                        if (GuiState._selectedScreenObjectState.IsScreenSurface)
                            ImGui.Text(((IScreenSurface)GuiState._selectedScreenObject).Surface.Width.ToString());
                        else
                            ImGui.Text("??");

                        ///////
                        // Position Y
                        ///////
                        ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Position Y: ");
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(100);
                        if (ImGui.InputInt("##y", ref GuiState._selectedScreenObjectState.PositionY))
                        {
                            GuiState._selectedScreenObject.Position = GuiState._selectedScreenObject.Position.WithY(GuiState._selectedScreenObjectState.PositionY);
                            GuiState._selectedScreenObjectState.Refresh();
                        }

                        ///////
                        // Height
                        ///////
                        ImGui.SameLine();
                        ImGui.TextColored(Color.AnsiCyanBright.ToVector4(), "Height: ");
                        ImGui.SameLine();
                        ImGui.SetNextItemWidth(80);
                        if (GuiState._selectedScreenObjectState.IsScreenSurface)
                            ImGui.Text(((IScreenSurface)GuiState._selectedScreenObject).Surface.Height.ToString());
                        else
                            ImGui.Text("??");

                        ImGui.Separator();
                    }
                    ImGui.EndChild();
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
}
