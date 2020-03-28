using System;
using SadConsole;
using System.Collections.Generic;
using SadRogue.Primitives;
using SadConsoleEditor.Tools;
using SadConsoleEditor.Panels;
using SadConsole.Input;
using SadConsole.UI;

namespace SadConsoleEditor.Consoles
{
    public class ToolPane : ControlsConsole
    {
        private ControlsConsole ScrollerConsole;
        private bool _isInitialized;

        public static int PanelWidth;
        public static int PanelWidthControls;

        public SadConsole.UI.Controls.ScrollBar ToolsPaneScroller { get; private set; }

        private List<Tuple<CustomPanel, int>> _hotSpots;
        public Dictionary<string, ITool> Tools;
        private List<(int, int, string)> _redrawCommands;

        public FilesPanel PanelFiles;

        internal ToolPane(): base(Config.Program.ToolPaneWidth - 1, Config.Program.WindowHeight * 3)
        {
            //ToolsConsole.MouseHandler = ProcessMouse;
            UseKeyboard = false;

            // Create scrollbar
            ToolsPaneScroller = new SadConsole.UI.Controls.ScrollBar(SadConsole.Orientation.Vertical, Config.Program.WindowHeight - 1);
            ToolsPaneScroller.Maximum = Height - Config.Program.WindowHeight;
            ToolsPaneScroller.ValueChanged += (o, e) =>
            {
                View = new Rectangle(0, ToolsPaneScroller.Value, Width, Config.Program.WindowHeight);
            };

            ScrollerConsole = new ControlsConsole(1, Config.Program.WindowHeight - 1);
            ScrollerConsole.Add(ToolsPaneScroller);
            ScrollerConsole.Position = new Point(Width, 0);
            ScrollerConsole.IsVisible = true;
            ScrollerConsole.FocusOnMouseClick = false;

            PanelWidth = Config.Program.ToolPaneWidth - 1;
            PanelWidthControls = PanelWidth - 2;

            Tools = new Dictionary<string, ITool>();
            
            _hotSpots = new List<Tuple<CustomPanel, int>>();

            View = new Rectangle(0, 0, Width, Config.Program.WindowHeight - 1);

            // Create panels
            PanelFiles = new FilesPanel();

            Children.Add(ScrollerConsole);

            _redrawCommands = new List<(int, int, string)>();
            _isInitialized = true;
        }

        internal void RegisterTool(ITool tool)
        {
            Tools.Add(tool.Id, tool);
        }

        public ITool GetTool(string id)
        {
            if (Tools.ContainsKey(id))
                return Tools[id];
            else
                throw new Exception($"Tool is not registered: {id}");
        }

        public IEnumerable<ITool> GetTools(params string[] ids)
        {
            foreach (var id in ids)
            {
                if (Tools.ContainsKey(id))
                    yield return Tools[id];
            }
        }

        protected override void OnThemeDrawn()
        {
            if (!_isInitialized) return;

            foreach (var item in _redrawCommands)
                this.Print(item.Item1, item.Item2, item.Item3);
        }

        public void RedrawPanels()
        {
            int activeRow = 0;
            //this.Clear();
            _redrawCommands.Clear();
            RemoveAll();
            _hotSpots.Clear();

            char open = (char)31;
            char closed = (char)16;

            List<CustomPanel> allPanels = new List<CustomPanel>() { PanelFiles };

            // Custom panels from the selected editor
            if (MainConsole.Instance.ActiveEditor != null)
                if (MainConsole.Instance.ActiveEditor.Panels != null && MainConsole.Instance.ActiveEditor.Panels.Length != 0)
                    allPanels.AddRange(MainConsole.Instance.ActiveEditor.Panels);

            // Custom panels from the selected tool
            //if (PanelTools.SelectedTool.ControlPanels != null && PanelTools.SelectedTool.ControlPanels.Length != 0)
            //    allPanels.AddRange(PanelTools.SelectedTool.ControlPanels);

            // Display all panels needed
            if (allPanels.Count != 0)
            {
                foreach (var pane in allPanels)
                {
                    if (pane.IsVisible)
                    {
                        pane.Loaded();
                        _hotSpots.Add(new Tuple<CustomPanel, int>(pane, activeRow));
                        if (pane.IsCollapsed == false)
                        {
                            _redrawCommands.Add((1, activeRow++, $"{open} {pane.Title}"));
                            _redrawCommands.Add((0, activeRow++, new string((char)196, Width)));

                            foreach (var control in pane.Controls)
                            {
                                if (control != null)
                                {
                                    if (control.IsVisible)
                                    {
                                        Add(control);
                                        control.Position = new Point(1, activeRow);
                                        activeRow += pane.Redraw(control) + control.Height;
                                    }
                                }
                                else
                                    activeRow++;
                            }

                            activeRow += 1;
                        }
                        else
                            _redrawCommands.Add((1, activeRow++, $"{closed} {pane.Title}"));
                    }
                }
            }

            int scrollAbility = activeRow + 1 - Config.Program.WindowHeight;

            if (scrollAbility <= 0)
            {
                ToolsPaneScroller.Value = 0;
                ToolsPaneScroller.IsEnabled = false;
                ToolsPaneScroller.Maximum = 0;
            }
            else
            {
                ToolsPaneScroller.Maximum = scrollAbility;
                ToolsPaneScroller.IsEnabled = true;
            }

            RedrawTheme();
        }
        
        public override bool ProcessMouse(MouseScreenObjectState state)
        {
            var scrollerState = new MouseScreenObjectState(ScrollerConsole, state.Mouse);

            if (state.IsOnScreenObject)
            {
                if (state.Mouse.ScrollWheelValueChange != 0)
                {
                    if (ToolsPaneScroller.IsEnabled)
                        ToolsPaneScroller.Value += state.Mouse.ScrollWheelValueChange;

                    return true;
                }

                foreach (var item in _hotSpots)
                {
                    if (item.Item2 == state.CellPosition.Y)
                    {
                        if (state.Mouse.LeftClicked)
                        {
                            item.Item1.IsCollapsed = !item.Item1.IsCollapsed;
                            RedrawPanels();
                            return true;
                        }
                    }
                }

                return base.ProcessMouse(state);
            }

            if (scrollerState.IsOnScreenObject)
                return ScrollerConsole.ProcessMouse(scrollerState);

            return false;
        }
        public override bool ProcessKeyboard(Keyboard info) =>
            false;
    }
}
