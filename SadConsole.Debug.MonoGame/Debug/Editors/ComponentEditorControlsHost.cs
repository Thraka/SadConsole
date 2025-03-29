using System.Linq;
using Hexa.NET.ImGui;
using SadConsole.Components;
using SadConsole.ImGuiSystem;
using SadConsole.UI;
using SadConsole.UI.Controls;

namespace SadConsole.Debug.Editors;

internal class ComponentEditorControlHost : ISadComponentPanel
{
    private ControlHost _stateComponent;
    private IScreenObject _screenObject;

    ImGuiList<ControlWrapper> _controlsList;

    public void BuildUI(ImGuiRenderer renderer, ScreenObjectState state, IComponent component)
    {
        // Capture the state information of the component as it is
        if (_screenObject != state.Object ||
            _stateComponent != state.Object.SadComponents[state.ComponentsSelectedItem])
        {
            _screenObject = state.Object;
            _stateComponent = (ControlHost)state.Object.SadComponents[state.ComponentsSelectedItem];
            _controlsList = new ImGuiList<ControlWrapper>(_stateComponent.Select((c) => new ControlWrapper(c)));
        }

        // Draw the ImGUI interface
        ImGui.BeginGroup();
        {
            ImGui.Text($"Controls: {_stateComponent.Count}");

            ImGui.SetNextItemWidth(-0);
            if (ImGui.ListBox("##controls", ref _controlsList.SelectedItemIndex, _controlsList.Names, _controlsList.Count, _controlsList.Count <= 5 ? 5 : 10))
            {

            }



            //ImGui.Checkbox("Visible", true)

        }
        ImGui.EndGroup();

        ImGui.Begin(GuiDockspace.ID_CENTER_PANEL, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar);
        {
            if (ImGui.BeginTabBar("preview_tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
            {
                if (ImGui.BeginTabItem("Controls Preview", ImGuiTabItemFlags.NoCloseWithMiddleMouseButton))
                {
                    ImGui.Text($"Controls: {_stateComponent.Count}");

                    

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
        ImGui.End();
    }

    private class ControlWrapper: ITitle
    {
        public ControlBase Control;
        public string Title { get; }

        public bool IsVisible;
        public bool IsEnabled;
        public int Width;
        public int Height;
        public int X;
        public int Y;
        public bool IsFocused;

        public ControlWrapper(ControlBase control)
        {
            Control = control;
            Title = control.Name ?? control.GetDebuggerDisplayValue();

            IsVisible = Control.IsVisible;
            IsEnabled = Control.IsEnabled;
            Width = control.Width;
            Height = control.Height;
            X = control.Position.X;
            Y = control.Position.Y;
            IsFocused = control.IsFocused;
        }

        public void Reflect()
        {
            Control.IsVisible = IsVisible;
            Control.IsEnabled = IsEnabled;

            if (Control.CanResize && (Control.Width != Width || Control.Height != Height))
                Control.Resize(Width, Height);

            if (Control.Position.X != X || Control.Position.Y != Y)
                Control.Position = new(X, Y);

            Control.IsFocused = IsFocused;
        }
    }
}
